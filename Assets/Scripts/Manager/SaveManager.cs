using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    [SerializeField] private ItemDatabase itemDatabase;

    private string savePath;

    private void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "save.json");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
            SaveGame();

        if (Input.GetKeyDown(KeyCode.F9))
            LoadGame();
    }

    public void SaveGame()
    {
        if (!HasRequiredDependencies())
            return;

        GameSaveData saveData = new();

        SaveGold(saveData);
        SaveInventory(saveData);
        SaveQuests(saveData);

        try
        {
            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(savePath, json);
        }
        catch (System.Exception ex) when (
            ex is IOException ||
            ex is System.UnauthorizedAccessException ||
            ex is System.Security.SecurityException ||
            ex is System.ArgumentException ||
            ex is System.NotSupportedException)
        {
            Debug.LogWarning($"Save failed ({ex.GetType().Name}).");
            return;
        }

        Debug.Log($"게임 저장 완료: {savePath}");
    }

    public void LoadGame()
    {
        if (!HasRequiredDependencies())
            return;

        if (!File.Exists(savePath))
        {
            Debug.LogWarning("저장 파일이 없습니다.");
            return;
        }

        GameSaveData saveData;
        try
        {
            string json = File.ReadAllText(savePath);
            saveData = JsonUtility.FromJson<GameSaveData>(json);
        }
        catch (System.Exception ex) when (
            ex is IOException ||
            ex is System.UnauthorizedAccessException ||
            ex is System.Security.SecurityException ||
            ex is System.ArgumentException ||
            ex is System.NotSupportedException)
        {
            Debug.LogWarning($"Load failed ({ex.GetType().Name}).");
            return;
        }

        if (saveData == null)
        {
            Debug.LogWarning("저장 데이터를 불러오지 못했습니다.");
            return;
        }

        if (saveData.inventory == null || saveData.quests == null)
        {
            Debug.LogWarning("Save data contains a null inventory or quest list.");
            return;
        }

        LoadGold(saveData);
        LoadInventory(saveData);
        LoadQuests(saveData);

        Debug.Log("게임 불러오기 완료");
    }

    private bool HasRequiredDependencies()
    {
        if (PlayerWallet.Instance == null ||
            PlayerInventory.Instance == null ||
            QuestManager.Instance == null ||
            itemDatabase == null)
        {
            Debug.LogWarning("Save/Load requires Wallet, Inventory, QuestManager and ItemDatabase.");
            return false;
        }

        return true;
    }

    private void SaveGold(GameSaveData saveData)
    {
        if (PlayerWallet.Instance != null)
            saveData.gold = PlayerWallet.Instance.Gold;
    }

    private void SaveInventory(GameSaveData saveData)
    {
        if (PlayerInventory.Instance == null)
            return;

        foreach (InventoryEntry entry in PlayerInventory.Instance.Entries)
        {
            if (entry == null || entry.ItemData == null)
                continue;

            saveData.inventory.Add(
                new InventorySaveData(entry.ItemData.ItemId, entry.Amount)
            );
        }
    }

    private void SaveQuests(GameSaveData saveData)
    {
        if (QuestManager.Instance == null)
            return;

        foreach (QuestData quest in QuestManager.Instance.Quests)
        {
            if (quest == null)
                continue;

            saveData.quests.Add(
                new QuestSaveData(
                    quest.QuestId,
                    quest.State,
                    quest.CurrentAmount
                )
            );
        }
    }

    private void LoadGold(GameSaveData saveData)
    {
        if (PlayerWallet.Instance != null)
            PlayerWallet.Instance.SetGold(saveData.gold);
    }

    private void LoadInventory(GameSaveData saveData)
    {
        if (PlayerInventory.Instance == null)
            return;

        PlayerInventory.Instance.Clear();

        foreach (InventorySaveData itemSave in saveData.inventory)
        {
            if (itemSave == null || string.IsNullOrWhiteSpace(itemSave.itemId))
            {
                Debug.LogWarning("Skipping inventory entry with a missing item ID.");
                continue;
            }

            ItemData itemData = itemDatabase.GetItem(itemSave.itemId);

            if (itemData == null)
            {
                Debug.LogWarning($"ItemData를 찾을 수 없습니다: {itemSave.itemId}");
                continue;
            }

            PlayerInventory.Instance.AddItem(itemData, itemSave.amount);
        }
    }

    private void LoadQuests(GameSaveData saveData)
    {
        if (QuestManager.Instance == null)
            return;

        foreach (QuestSaveData questSave in saveData.quests)
        {
            if (questSave == null || string.IsNullOrWhiteSpace(questSave.questId))
            {
                Debug.LogWarning("Skipping quest entry with a missing quest ID.");
                continue;
            }

            bool loaded = QuestManager.Instance.LoadQuestState(
                questSave.questId,
                questSave.state,
                questSave.currentAmount
            );

            if (!loaded)
                Debug.LogWarning($"퀘스트를 찾을 수 없습니다: {questSave.questId}");
        }
    }
}