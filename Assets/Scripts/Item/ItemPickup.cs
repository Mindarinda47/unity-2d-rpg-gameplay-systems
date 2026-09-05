using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    [Header("Item")]
    [SerializeField] private ItemData itemData;

    [Min(1)]
    [SerializeField] private int amount = 1;

    [Header("Interaction")]
    [SerializeField] private string interactionText = "줍기";
    [SerializeField] private bool hideWhenEmpty = true;

    private bool isCollected;

    public void Interact()
    {
        if (isCollected)
            return;

        if (itemData == null)
        {
            Debug.LogWarning(
                $"{gameObject.name}에 ItemData가 연결되지 않았습니다."
            );

            return;
        }

        if (PlayerInventory.Instance == null)
        {
            Debug.LogWarning(
                "씬에서 PlayerInventory를 찾을 수 없습니다."
            );

            return;
        }

        int acceptedAmount =
            PlayerInventory.Instance.AddItem(
                itemData,
                amount
            );

        if (acceptedAmount <= 0)
        {
            Debug.Log(
                $"{itemData.ItemName}을 더 이상 획득할 수 없습니다."
            );

            return;
        }

        amount -= acceptedAmount;

        Debug.Log(
            $"필드 아이템 습득: {itemData.ItemName} " +
            $"+{acceptedAmount}, 남은 수량: {amount}"
        );

        if (amount <= 0)
        {
            CollectAll();
        }
    }

    private void CollectAll()
    {
        isCollected = true;
        amount = 0;

        if (hideWhenEmpty)
        {
            gameObject.SetActive(false);
        }
    }

    public void EndInteract()
    {
    }

    public string GetInteractionText()
    {
        if (itemData == null)
            return interactionText;

        if (amount > 1)
        {
            return $"{interactionText}"; // 필요 시 아이템 이름 삽입 가능
        }

        return $"{interactionText}"; // 필요 시 아이템 이름 삽입 가능
    }

    public bool IsDialogue()
    {
        return false;
    }
}
