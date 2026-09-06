using System;
using System.Collections.Generic;

// Data Transfer Object - DTO
// 데이터를 옮기기 위한 객체
[Serializable]
public class GameSaveData
{
    public int gold;
    public List<InventorySaveData> inventory = new();
    public List<QuestSaveData> quests = new();
}

[Serializable]
public class InventorySaveData
{
    public string itemId;
    public int amount;

    public InventorySaveData(string itemId, int amount)
    {
        this.itemId = itemId;
        this.amount = amount;
    }
}

[Serializable]
public class QuestSaveData
{
    public string questId;
    public QuestState state;
    public int currentAmount;

    public QuestSaveData(string questId, QuestState state, int currentAmount)
    {
        this.questId = questId;
        this.state = state;
        this.currentAmount = currentAmount;
    }
}