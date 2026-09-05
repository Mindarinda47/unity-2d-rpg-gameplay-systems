using UnityEngine;

public class PlayerItemUse : MonoBehaviour
{
    private PlayerInventory inventory;
    private PlayerHealth health;

    private void Awake()
    {
        inventory = GetComponent<PlayerInventory>();
        health = GetComponent<PlayerHealth>();
    }

    public bool TryUseItem(InventoryEntry entry)
    {
        if (entry == null || entry.ItemData == null)
            return false;

        ConsumableItemData consumable = entry.ItemData as ConsumableItemData;

        if (consumable == null)
        {
            Debug.Log($"{entry.ItemData.ItemName}은 소비 아이템이 아닙니다.");
            return false;
        }

        int healedAmount = health.Heal(consumable.HealAmount);

        if (healedAmount <= 0)
        {
            Debug.Log("HP가 가득 차 있어 아이템을 사용하지 않았습니다.");
            return false;
        }

        inventory.RemoveItem(entry.ItemData.ItemId, 1);

        return true;
    }
}