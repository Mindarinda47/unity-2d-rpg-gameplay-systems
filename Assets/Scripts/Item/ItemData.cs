using UnityEngine;

[CreateAssetMenu(
    fileName = "NewItemData",
    menuName = "Game Data/Item Data"
)]
public class ItemData : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string itemId;
    [SerializeField] private string itemName;
    [SerializeField] private ItemType itemType;

    [Header("Display")]
    [TextArea(2, 4)]
    [SerializeField] private string description;
    [SerializeField] private Sprite icon;

    [Header("Stack")]
    [Min(1)]
    [SerializeField] private int maxStack = 99;

    public string ItemId => itemId;
    public string ItemName => itemName;
    public ItemType ItemType => itemType;
    public string Description => description;
    public Sprite Icon => icon;
    public int MaxStack => Mathf.Max(1, maxStack);
}
