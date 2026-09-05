using UnityEngine;

public interface IInteractable
{
    void Interact();
    void EndInteract();
    string GetInteractionText();
    bool IsDialogue();
}