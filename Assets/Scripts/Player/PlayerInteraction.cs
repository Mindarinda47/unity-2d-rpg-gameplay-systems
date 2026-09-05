using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    private IInteractable currentInteractable;
    private bool isInteracting;
    private PlayerMovement playerMovement;
    [SerializeField] private GameObject interactionUI;
    [SerializeField] private GameObject dialogueUI;
    [SerializeField] private TextMeshProUGUI interactionText;
    [SerializeField] private DialogueManager dialogueManager;

    private void Awake()
    {
        interactionUI.SetActive(false);

        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (playerMovement == null || playerMovement.IsDead)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentInteractable == null)
                return;

            if (currentInteractable.IsDialogue())
            {
                HandleDialogue();
            }
            else
            {
                currentInteractable.Interact();

                currentInteractable = null;
                interactionUI.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable))
        {
            currentInteractable = interactable;
            Debug.Log("E를 눌러 상호작용");
            interactionUI.SetActive(true);
            interactionText.text = $"E : {interactable.GetInteractionText()}";
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable))
        {
            if (currentInteractable == interactable)
            {
                Debug.Log("상호작용 범위 벗어남");
                currentInteractable = null;
                interactionUI.SetActive(false);
            }
        }
    }

    private void HandleDialogue()
    {
        if (currentInteractable is NPCInteractable npc)
        {
            if (isInteracting == false)
            {
                isInteracting = true;

                dialogueManager.Open(npc);

                playerMovement.SetTalking(true);

                interactionUI.SetActive(false);
            }
            else
            {
                bool isContinue = dialogueManager.NextDialogue();

                if (isContinue == false)
                {
                    isInteracting = false;

                    playerMovement.SetTalking(false);

                    if (currentInteractable != null)
                    {
                        interactionUI.SetActive(true);
                    }
                }
            }
        }
    }
}
