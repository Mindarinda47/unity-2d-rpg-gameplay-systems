using UnityEngine;

public class ChestInteractable : MonoBehaviour, IInteractable
{
    private bool isOpened = false;

    public void Interact()
    {
        if (isOpened)
        {
            Debug.Log("이미 열린 상자");
            return;
        }

        isOpened = true;
        Debug.Log("상자열림!");
    }

    public void EndInteract()
    {
        Debug.Log("상자는 닫지 않음 (무시)");
    }

    public string GetInteractionText()
    {
        return "열기";
    }

    public bool IsDialogue()
    {
        return false;
    }
}
