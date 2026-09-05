using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Game Data/Item Database")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] private List<ItemData> items = new();

    public ItemData GetItem(string itemId)
    {
        foreach (ItemData item in items)
        {
            if (item != null && item.ItemId == itemId)
                return item;
        }

        return null;
    }
}