using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Transform slotParent;
    [SerializeField] private InventorySlotUI slotPrefab;
    [SerializeField] private TMP_Text emptyText;
    [SerializeField] private InventoryItemDetailUI detailUI;
    [SerializeField] private PlayerItemUse playerItemUse;

    [Header("Input")]
    [SerializeField] private KeyCode toggleKey = KeyCode.I;

    private PlayerInventory playerInventory;
    private readonly List<InventorySlotUI> slotViews = new();
    private InventoryEntry selectedEntry;

    private bool isOpen;

    private void Start()
    {
        playerInventory = PlayerInventory.Instance;

        if (playerInventory == null)
        {
            Debug.LogWarning("InventoryUI가 PlayerInventory를 찾지 못했습니다.");
            return;
        }

        playerInventory.InventoryChanged += HandleInventoryChanged;
        playerInventory.InventoryReset += HandleInventoryReset;
        detailUI.UseRequested += HandleUseRequested;

        Close();
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
            Toggle();
    }

    private void OnDestroy()
    {
        if (playerInventory != null)
        {
            playerInventory.InventoryChanged -= HandleInventoryChanged;
            playerInventory.InventoryReset -= HandleInventoryReset;
        }
        if (detailUI != null)
            detailUI.UseRequested -= HandleUseRequested;
    }

    private void HandleInventoryChanged(ItemData changedItem, int currenAmount)
    {
        if (isOpen)
            Refresh();
    }

    private void HandleInventoryReset()
    {
        if (isOpen)
            Refresh();
    }

    public void Toggle()
    {
        if (isOpen)
            Close();
        else
            Open();
    }

    public void Open()
    {
        isOpen = true;
        selectedEntry = null;

        inventoryPanel.SetActive(true);
        detailUI.Clear();
        Refresh();
    }

    public void Close()
    {
        isOpen = false;
        inventoryPanel.SetActive(false);
    }

    private void Refresh()
    {
        if (playerInventory == null)
            return;

        IReadOnlyList<InventoryEntry> entries = playerInventory.Entries;

        emptyText.gameObject.SetActive(entries.Count == 0);

        if (entries.Count == 0)
            detailUI.Clear();

        for (int i = 0; i < entries.Count; i++)
        {
            InventorySlotUI slotView = GetOrCreateSlot(i);

            slotView.gameObject.SetActive(true);
            slotView.Bind(entries[i]);
        }

        for (int i = entries.Count; i < slotViews.Count; i++)
            slotViews[i].gameObject.SetActive(false);

        if (selectedEntry != null)
        {
            if (playerInventory.ContainsEntry(selectedEntry))
            {
                detailUI.Show(selectedEntry);
            }
            else
            {
                selectedEntry = null;
                detailUI.Clear();
            }
        }
    }

    private InventorySlotUI GetOrCreateSlot(int index)
    {
        if (index < slotViews.Count)
            return slotViews[index];

        InventorySlotUI newSlot = Instantiate(slotPrefab, slotParent);

        newSlot.Selected += HandleSlotSelected;

        slotViews.Add(newSlot);

        return newSlot;
    }

    private void HandleSlotSelected(InventoryEntry entry)
    {
        selectedEntry = entry;
        detailUI.Show(entry);
    }

    private void HandleUseRequested()
    {
        if (selectedEntry == null || playerItemUse == null)
            return;

        playerItemUse.TryUseItem(selectedEntry);
    }
}
