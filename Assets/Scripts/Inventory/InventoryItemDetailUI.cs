using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemDetailUI : MonoBehaviour
{
    [SerializeField] private GameObject detailPanel;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text itemTypeText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private Button useButton;

    public event Action UseRequested;

    private void Awake()
    {
        useButton.onClick.AddListener(HandleUseClicked);
    }

    private void OnDestroy()
    {
        useButton.onClick.RemoveListener(HandleUseClicked);
    }

    public void Show(InventoryEntry entry)
    {
        if (entry == null || entry.ItemData == null)
        {
            Clear();
            return;
        }

        ItemData itemData = entry.ItemData;

        detailPanel.SetActive(true);

        itemIcon.sprite = itemData.Icon;
        itemIcon.enabled = itemData.Icon != null;

        itemNameText.text = itemData.ItemName;
        itemTypeText.text = itemData.ItemType.ToString();
        descriptionText.text = itemData.Description;
        amountText.text = $"º¸À¯: {entry.Amount}";

        useButton.gameObject.SetActive(itemData is ConsumableItemData);
    }

    public void Clear()
    {
        detailPanel.SetActive(false);
    }

    private void HandleUseClicked()
    {
        UseRequested?.Invoke();
    }
}