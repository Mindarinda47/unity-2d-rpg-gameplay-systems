using UnityEngine;

[CreateAssetMenu(
    fileName = "NewConsumableItem",
    menuName = "Game Data/Consumable Item"
)]
public class ConsumableItemData : ItemData
{
    [Header("Consumable")]
    [Min(1)]
    [SerializeField] private int healAmount = 30;

    public int HealAmount => healAmount;
}