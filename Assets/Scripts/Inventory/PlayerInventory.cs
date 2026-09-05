using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    [Header("Runtime Inventory")]
    [SerializeField]
    private List<InventoryEntry> entries = new();

    public IReadOnlyList<InventoryEntry> Entries => entries;

    public event Action<ItemData, int> InventoryChanged;
    public event Action InventoryReset;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning(
                "PlayerInventory가 중복으로 존재합니다."
            );

            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public int AddItem(ItemData itemData, int amount = 1)
    {
        if (itemData == null)
        {
            Debug.LogWarning(
                "추가하려는 ItemData가 없습니다."
            );

            return 0;
        }

        if (string.IsNullOrWhiteSpace(itemData.ItemId))
        {
            Debug.LogWarning(
                $"아이템 ID가 비어 있습니다: {itemData.name}"
            );

            return 0;
        }

        if (amount <= 0)
        {
            Debug.LogWarning(
                $"추가 수량은 1 이상이어야 합니다: {amount}"
            );

            return 0;
        }

        InventoryEntry entry = FindEntry(itemData.ItemId);

        int acceptedAmount;

        if (entry == null)
        {
            acceptedAmount =
                Mathf.Min(amount, itemData.MaxStack);

            entry = new InventoryEntry(
                itemData,
                acceptedAmount
            );

            entries.Add(entry);
        }
        else
        {
            acceptedAmount = entry.AddAmount(amount);
        }

        if (acceptedAmount <= 0)
        {
            Debug.Log(
                $"{itemData.ItemName}의 최대 보유 수량에 도달했습니다."
            );

            return 0;
        }

        InventoryChanged?.Invoke(
            itemData,
            entry.Amount
        );

        Debug.Log(
            $"아이템 획득: {itemData.ItemName} " +
            $"+{acceptedAmount} / 현재 {entry.Amount}"
        );

        return acceptedAmount;
    }

    public bool RemoveItem(string itemId, int amount = 1)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            Debug.LogWarning("제거할 아이템 ID가 비어 있습니다.");
            return false;
        }

        if (amount <= 0)
        {
            Debug.LogWarning(
                $"제거 수량은 1 이상이어야 합니다: {amount}"
            );

            return false;
        }

        InventoryEntry entry = FindEntry(itemId);

        if (entry == null)
        {
            Debug.LogWarning(
                $"보유하고 있지 않은 아이템입니다: {itemId}"
            );

            return false;
        }

        bool removeSucceeded =
            entry.RemoveAmount(amount);

        if (removeSucceeded == false)
        {
            Debug.Log(
                $"아이템 수량이 부족합니다: " +
                $"{entry.ItemData.ItemName} " +
                $"{entry.Amount}/{amount}"
            );

            return false;
        }

        ItemData removedItemData = entry.ItemData;
        int remainingAmount = entry.Amount;

        if (entry.IsEmpty())
        {
            entries.Remove(entry);
        }

        InventoryChanged?.Invoke(
            removedItemData,
            remainingAmount
        );

        Debug.Log(
            $"아이템 제거: {removedItemData.ItemName} " +
            $"-{amount} / 현재 {remainingAmount}"
        );

        return true;
    }

    public int GetItemAmount(string itemId)
    {
        InventoryEntry entry = FindEntry(itemId);

        if (entry == null)
        {
            return 0;
        }

        return entry.Amount;
    }

    public bool HasItem(string itemId, int requiredAmount = 1)
    {
        if (requiredAmount <= 0)
        {
            return false;
        }

        return GetItemAmount(itemId) >= requiredAmount;
    }

    private InventoryEntry FindEntry(string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            return null;
        }

        foreach (InventoryEntry entry in entries)
        {
            if (entry == null ||
                entry.ItemData == null)
            {
                continue;
            }

            if (entry.ItemData.ItemId == itemId)
            {
                return entry;
            }
        }

        return null;
    }

    #if UNITY_EDITOR
        [SerializeField] private ItemData testItemData;

        [ContextMenu("Test/Add One Item")]
        private void TestAddItem()
        {
            AddItem(testItemData, 1);
        }

        [ContextMenu("Test/Remove One Item")]
        private void TestRemoveItem()
        {
            if (testItemData == null)
                return;

            RemoveItem(testItemData.ItemId, 1);
        }
#endif

    public bool ContainsEntry(InventoryEntry entry)
    {
        return entry != null && entries.Contains(entry);
    }

    public void Clear()
    {
        entries.Clear();
        InventoryReset?.Invoke();
    }
}
