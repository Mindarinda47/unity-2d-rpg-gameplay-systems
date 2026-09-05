using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private Button button;

    private InventoryEntry boundEntry;

    public event Action<InventoryEntry> Selected;

    private void Awake()
    {
        button.onClick.AddListener(HandleClick);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(HandleClick);
    }

    public void Bind(InventoryEntry entry)
    {
        if (entry == null || entry.ItemData == null)
        {
            boundEntry = null;
            gameObject.SetActive(false);
            return;
        }

        boundEntry = entry;

        ItemData itemData = entry.ItemData;

        iconImage.sprite = itemData.Icon;
        iconImage.enabled = itemData.Icon != null;

        itemNameText.text = itemData.ItemName;
        amountText.text = $"¡¿{entry.Amount}";
    }

    private void HandleClick()
    {
        if (boundEntry == null)
            return;

        Selected?.Invoke(boundEntry);
    }
}