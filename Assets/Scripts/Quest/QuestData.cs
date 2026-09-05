using UnityEngine;

[System.Serializable]
public class QuestData
{
    [SerializeField] private string questId;
    [SerializeField] private string questName;

    [TextArea(2, 4)]
    [SerializeField] private string desctiption;

    [SerializeField] private QuestState state = QuestState.NotStarted;

    [Header("Reward")]
    [SerializeField] private int goldReward;

    [Header("Objective")]
    [TextArea(1, 3)]
    [SerializeField] private string objectiveDescription;

    [TextArea(1, 3)]
    [SerializeField] private string completedDescription;

    [SerializeField] private int requiredAmount = 1;
    [SerializeField] private int currentAmount;

    [Header("Collect Objective")]
    [SerializeField] private string requiredItemId;

    [Header("Objective Type")]
    [SerializeField] private QuestObjectiveType objectiveType;
    [SerializeField] private string targetId;

    public QuestObjectiveType ObjectiveType => objectiveType;
    public string TargetId => targetId;
    public string QuestId => questId;
    public string QuestName => questName;
    public string Desctiption => desctiption;
    public QuestState State => state;
    public int GoldReward => goldReward;
    public string ObjectiveDescription => objectiveDescription;
    public string CompletedDescription => completedDescription;
    public int RequiredAmount => Mathf.Max(1, requiredAmount); // 목표 수량 최소 1 반환 보호
    public int CurrentAmount => currentAmount;
    public string RequiredItemId => requiredItemId;

    public bool StartQuest()
    {
        if (state != QuestState.NotStarted)
            return false;

        currentAmount = 0;
        state = QuestState.InProgress;

        return true;
    }

    public bool CompleteQuest()
    {
        if (state != QuestState.InProgress)
            return false;

        state = QuestState.Completed;
        return true;
    }

    public bool RewardQuest()
    {
        if (state != QuestState.Completed)
            return false;

        state = QuestState.Rewarded;
        return true;
    }

    public bool IsNotStarted()
    {
        return state == QuestState.NotStarted;
    }

    public bool IsInProgress()
    {
        return (state == QuestState.InProgress);
    }

    public bool IsCompleted()
    {
        return state == QuestState.Completed;
    }

    public bool IsRewarded()
    {
        return state == QuestState.Rewarded;
    }

    public bool AddProgress(int amount)
    {
        if (state != QuestState.InProgress)
            return false;

        if (amount <= 0)
            return false;

        int previousAmount = currentAmount;

        // Clamp는 값이 지정 범위 넘어가지 않게 해줌.
        currentAmount = Mathf.Clamp(
            currentAmount + amount,
            0,
            RequiredAmount
        );

        if (currentAmount >= RequiredAmount)
        {
            state = QuestState.Completed;
        }

        return currentAmount != previousAmount;
    }

    public bool SetProgress(int newAmount)
    {
        if (state != QuestState.InProgress && state != QuestState.Completed)
        {
            return false;
        }

        int previousAmount = currentAmount;
        QuestState previousState = state;

        currentAmount = Mathf.Clamp(
            newAmount,
            0,
            RequiredAmount
        );

        state = currentAmount >= RequiredAmount
            ? QuestState.Completed
            : QuestState.InProgress;

        bool amountChanged = previousAmount != currentAmount;

        bool stateChanged = previousState != state;

        return amountChanged || stateChanged;
    }

    public void LoadState(QuestState savedState, int savedAmount)
    {
        state = savedState;
        currentAmount = Mathf.Clamp(savedAmount, 0, RequiredAmount);
    }
}