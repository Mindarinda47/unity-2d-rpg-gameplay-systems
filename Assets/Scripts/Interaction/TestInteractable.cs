using UnityEngine;

public class TestInteractable : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("상호작용 성공!");
    }
    public void EndInteract()
    {

    }
    public string GetInteractionText()
    {
        return "상호작용";
    }

    public bool IsDialogue()
    {
        return false;
    }
}