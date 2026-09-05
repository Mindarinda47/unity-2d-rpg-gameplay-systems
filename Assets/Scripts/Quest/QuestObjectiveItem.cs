using UnityEngine;

public class QuestObjectiveItem : MonoBehaviour, IInteractable
{
    [Header("Quest")]
    [SerializeField] private string questId;

    [Header("Interaction")]
    [SerializeField] private string interactionText = "줍기";
    [SerializeField] private bool hideAfterInteract = true;

    public void Interact()
    {
        if (QuestManager.Instance == null)
        {
            Debug.LogWarning("QuestManager가 씬에 없습니다.");
            return;
        }

        QuestState state = QuestManager.Instance.GetQuestState(questId);

        if (state != QuestState.InProgress)
        {
            Debug.Log($"진행 중인 퀘스트가 아닙니다. QuestId: {questId}, 현재 상태: {state}");
            return;
        }

        QuestManager.Instance.AddQuestProgress(questId, 1);

        Debug.Log($"퀘스트 목표 달성: {questId}");

        if (hideAfterInteract)
        {
            gameObject.SetActive(false);
        }
    }

    public void EndInteract()
    {

    }

    public string GetInteractionText()
    {
        return interactionText;
    }

    public bool IsDialogue()
    {
        return false;
    }
}
