using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    public event Action<QuestData, QuestState> QuestStateChanged;
    public event Action<QuestData, int, int> QuestProgressChanged;

    [SerializeField] private List<QuestData> quests = new List<QuestData>();

    private readonly Dictionary<string, QuestData> questMap = new Dictionary<string, QuestData>();
    private PlayerInventory playerInventory;

    public IReadOnlyList<QuestData> Quests => quests;

    private void Start()
    {
        playerInventory = PlayerInventory.Instance;

        if (playerInventory != null)
            playerInventory.InventoryChanged += HandleInventoryChanged;

        CombatEvents.EnemyKilled += HandleEnemyKilled;
        InteractionEvents.NpcTalkCompleted += HandleNpcTalkCompleted;
    }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        BuildQuestMap();
    }

    private void OnDestroy()
    {
        if (playerInventory != null)
            playerInventory.InventoryChanged -= HandleInventoryChanged;

        CombatEvents.EnemyKilled -= HandleEnemyKilled;
        InteractionEvents.NpcTalkCompleted -= HandleNpcTalkCompleted;

        if (Instance == this)
            Instance = null;
    }

    private void BuildQuestMap()
    {
        questMap.Clear();

        foreach (QuestData quest in quests)
        {
            if (quest == null)
                continue;

            if (string.IsNullOrEmpty(quest.QuestId))
            {
                Debug.LogWarning("Quest ID가 비어 있는 퀘스트가 있습니다.");
                continue;
            }

            if (questMap.ContainsKey(quest.QuestId))
            {
                Debug.LogWarning($"중복된 Quest ID가 있습니다: {quest.QuestId}");
                continue;
            }

            questMap.Add(quest.QuestId, quest);
        }
    }

    public bool TryGetQuest(string questId, out QuestData quest)
    {
        return questMap.TryGetValue(questId, out quest);
    }

    public bool TryGetTrackedQuest(out QuestData trackedQuest)
    {
        foreach (QuestData quest in quests)
        {
            if (quest == null)
                continue;

            if (quest.State == QuestState.InProgress ||
                quest.State == QuestState.Completed)
            {
                trackedQuest = quest;
                return true;
            }
        }

        trackedQuest = null;
        return false;
    }

    public void StartQuest(string questId)
    {
        if (TryGetQuest(questId, out QuestData quest) == false)
        {
            Debug.LogWarning($"퀘스트를 찾을 수 없습니다: {questId}");
            return;
        }

        bool startSucceeded = quest.StartQuest();

        if (startSucceeded == false)
        {
            Debug.LogWarning(
                $"퀘스트를 시작할 수 없는 상태입니다. " +
                $"퀘스트: {quest.QuestName}, 현재 상태: {quest.State}"
            );

            return;
        }

        QuestStateChanged?.Invoke(quest, quest.State);

        SynchronizeQuestWithInventory(quest);

        Debug.Log($"퀘스트 시작: {quest.QuestName} / 상태: {quest.State}");
    }

    public void CompleteQuest(string questId)
    {
        if (TryGetQuest(questId, out QuestData quest) == false)
        {
            Debug.LogWarning($"퀘스트를 찾을 수 없습니다: {questId}");
            return;
        }

        bool completeSucceeded = quest.CompleteQuest();

        if (completeSucceeded == false)
        {
            Debug.LogWarning(
                $"퀘스트를 완료할 수 없는 상태입니다. " +
                $"퀘스트: {quest.QuestName}, 현재 상태: {quest.State}"
            );

            return;
        }

        QuestStateChanged?.Invoke(quest, quest.State);

        Debug.Log($"퀘스트 완료: {quest.QuestName} / 상태: {quest.State}");
    }

    public void RewardQuest(string questId)
    {
        if (TryGetQuest(questId, out QuestData quest) == false)
        {
            Debug.LogWarning($"퀘스트를 찾을 수 없습니다: {questId}");
            return;
        }

        if (quest.State != QuestState.Completed)
        {
            Debug.LogWarning(
                $"보상을 받을 수 없는 상태입니다. " +
                $"퀘스트: {quest.QuestName}, 현재 상태: {quest.State}"
            );

            return;
        }

        if (PlayerWallet.Instance == null)
        {
            Debug.LogWarning("PlayerWallet이 씬에 없습니다.");
            return;
        }

        // 수집 퀘스트라면 필요한 아이템을 실제로 가지고 있는지 확인
        if (!string.IsNullOrWhiteSpace(quest.RequiredItemId))
        {
            if (PlayerInventory.Instance == null)
            {
                Debug.LogWarning("PlayerInventory가 씬에 없습니다.");
                return;
            }

            bool hasRequiredItems = PlayerInventory.Instance.HasItem(
                quest.RequiredItemId,
                quest.RequiredAmount
            );

            if (!hasRequiredItems)
            {
                Debug.LogWarning(
                    $"퀘스트 아이템이 부족합니다. " +
                    $"퀘스트: {quest.QuestName}"
                );

                return;
            }
        }

        bool rewardSucceeded = quest.RewardQuest();

        if (rewardSucceeded == false)
        {
            Debug.LogWarning($"퀘스트 보상 처리에 실패했습니다: {quest.QuestName}");
            return;
        }

        // Rewarded 상태로 먼저 만든 뒤 아이템 제거
        if (!string.IsNullOrWhiteSpace(quest.RequiredItemId))
        {
            PlayerInventory.Instance.RemoveItem(
                quest.RequiredItemId,
                quest.RequiredAmount
            );
        }

        PlayerWallet.Instance.AddGold(quest.GoldReward);

        QuestStateChanged?.Invoke(quest, quest.State);

        Debug.Log(
            $"퀘스트 보상 수령: {quest.QuestName} / " +
            $"지급 골드: {quest.GoldReward} / " +
            $"상태: {quest.State}"
        );
    }

    public QuestState GetQuestState(string questId)
    {
        if (TryGetQuest(questId, out QuestData quest) == false)
        {
            Debug.LogWarning($"퀘스트를 찾을 수 없습니다: {questId}");
            return QuestState.NotStarted;
        }

        return quest.State;
    }

    public void AddQuestProgress(string questId, int amount = 1)
    {
        if (TryGetQuest(questId, out QuestData quest) == false)
        {
            Debug.LogWarning($"퀘스트를 찾을 수 없습니다: {questId}");
            return;
        }

        if (amount <= 0)
        {
            Debug.LogWarning($"올바르지 않은 퀘스트 진행도입니다: {amount}");
        }

        QuestState previousState = quest.State;

        bool progressSucceeded = quest.AddProgress(amount);

        if (progressSucceeded == false)
        {
            Debug.LogWarning(
                $"퀘스트 진행도를 증가시킬 수 없습니다. " +
                $"퀘스트: {quest.QuestName}, 상태: {quest.State}"
            );

            return;
        }

        QuestProgressChanged?.Invoke(
            quest,
            quest.CurrentAmount,
            quest.RequiredAmount
        );

        if (previousState != quest.State)
        {
            QuestStateChanged?.Invoke(quest, quest.State);
        }

        Debug.Log(
            $"퀘스트 진행: {quest.QuestName} / " +
            $"{quest.CurrentAmount}/{quest.RequiredAmount} / " +
            $"상태: {quest.State}"
        );
    }

    private void HandleInventoryChanged(ItemData changedItem, int currentAmount)
    {
        if (changedItem == null)
            return;

        foreach (QuestData quest in quests)
        {
            if (quest == null)
                continue;

            if (quest.State != QuestState.InProgress &&
                quest.State != QuestState.Completed)
            {
                continue;
            }

            if (quest.RequiredItemId != changedItem.ItemId)
                continue;

            SynchronizeQuestProgress(quest, currentAmount);
        }
    }

    private void SynchronizeQuestProgress(QuestData quest, int inventoryAmount)
    {
        if (quest == null)
            return;

        int previousAmount = quest.CurrentAmount;

        QuestState previousState = quest.State;

        bool changed = quest.SetProgress(inventoryAmount);

        if (changed == false)
            return;

        if (previousAmount != quest.CurrentAmount)
        {
            QuestProgressChanged?.Invoke(
                quest,
                quest.CurrentAmount,
                quest.RequiredAmount
            );
        }

        if (previousState != quest.State)
        {
            QuestStateChanged?.Invoke(
                quest,
                quest.State
            );
        }

        Debug.Log(
            $"퀘스트 진행도 동기화: " +
            $"{quest.QuestName} " +
            $"{quest.CurrentAmount}/" +
            $"{quest.RequiredAmount} " +
            $"상태: {quest.State}"
        );
    }

    private void SynchronizeQuestWithInventory(QuestData quest)
    {
        if (quest == null)
            return;

        if (string.IsNullOrWhiteSpace(quest.RequiredItemId))
        {
            return;
        }

        PlayerInventory inventory =
            playerInventory != null
                ? playerInventory
                : PlayerInventory.Instance;

        if (inventory == null)
        {
            Debug.LogWarning(
                "퀘스트 진행도를 동기화할 " +
                "PlayerInventory가 없습니다."
            );

            return;
        }

        int inventoryAmount = inventory.GetItemAmount(quest.RequiredItemId);

        SynchronizeQuestProgress(quest, inventoryAmount);
    }

    private void HandleEnemyKilled(string enemyId)
    {
        foreach (QuestData quest in quests)
        {
            if (quest == null)
                continue;

            if (quest.State != QuestState.InProgress)
                continue;

            if (quest.ObjectiveType != QuestObjectiveType.Kill)
                continue;

            if (quest.TargetId != enemyId)
                continue;

            AddQuestProgress(quest.QuestId, 1);
        }
    }

    private void HandleNpcTalkCompleted(string npcId)
    {
        foreach (QuestData quest in quests)
        {
            if (quest == null)
                continue;

            if (quest.State != QuestState.InProgress)
                continue;

            if (quest.ObjectiveType != QuestObjectiveType.Talk)
                continue;

            if (quest.TargetId != npcId)
                continue;

            AddQuestProgress(quest.QuestId, 1);
        }
    }

    public bool LoadQuestState(string questId, QuestState state, int currentAmount)
    {
        if (!TryGetQuest(questId, out QuestData quest))
            return false;

        quest.LoadState(state, currentAmount);

        QuestProgressChanged?.Invoke(quest, quest.CurrentAmount, quest.RequiredAmount);
        QuestStateChanged?.Invoke(quest, quest.State);

        return true;
    }
}
