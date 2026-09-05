using System;
using UnityEngine;

[Serializable]
public class InventoryEntry
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private int amount;

    public ItemData ItemData => itemData;
    public int Amount => amount;

    // 생성자의 중요한 역할은: 객체가 태어나는 순간부터 유효한 상태가 되도록 보장하는 것
    public InventoryEntry(ItemData itemData, int amount)
    {
        this.itemData = itemData;

        this.amount = itemData == null
            ? 0
            : Mathf.Clamp(amount, 0, itemData.MaxStack);
    }

    public int AddAmount(int addedAmount)
    {
        if (itemData == null || addedAmount <= 0)
            return 0;

        int availableSpace = itemData.MaxStack - amount;
        int acceptedAmount = Mathf.Min(addedAmount, availableSpace);

        amount += acceptedAmount;

        return acceptedAmount;
    }

    public bool RemoveAmount(int removedAmount)
    {
        if (removedAmount <= 0 || amount < removedAmount)
            return false;

        amount -= removedAmount;
        return true;
    }

    public bool IsEmpty()
    {
        return amount <= 0;
    }
}
