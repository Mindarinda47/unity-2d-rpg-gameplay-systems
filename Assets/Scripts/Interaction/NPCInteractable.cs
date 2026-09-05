using UnityEngine;

public class NPCInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string npcId;

    [Header("Default Dialogue")]
    [SerializeField] private DialogueLine[] dialogues;

    [Header("Quest Dialogue")]
    [SerializeField] private string relatedQuestId;

    [SerializeField] private DialogueLine[] inProgressDialogues;
    [SerializeField] private DialogueLine[] completedDialogues;
    [SerializeField] private DialogueLine[] rewardedDialogues;

    public string NpcId => npcId;

    private DialogueLine[] activeDialogues;
    private int index = 0;
    public void Interact()
    {

    }

    public void EndInteract()
    {
        index = 0;
        activeDialogues = null;

        if (!string.IsNullOrWhiteSpace(npcId))
            InteractionEvents.RaiseNpcTalkCompleted(npcId);
    }

    public string GetInteractionText()
    {
        return "대화하기";
    }

    public bool IsDialogue()
    {
        return true;
    }

    public bool IsDialogueEnd()
    {
        return activeDialogues == null || index >= activeDialogues.Length;
    }

    public DialogueLine GetNextDialogue()
    {
        if (activeDialogues == null || activeDialogues.Length == 0)
            return null;

        if (index >= activeDialogues.Length)
            return null;

        DialogueLine line = activeDialogues[index];
        index++;

        return line;
    }

    public bool IsValidDialogueIndex(int dialogueIndex)
    {
        return activeDialogues != null &&
               dialogueIndex >= 0 &&
               dialogueIndex < activeDialogues.Length;
    }

    public void SetDialogueIndex(int dialogueIndex)
    {
        if(IsValidDialogueIndex(dialogueIndex) == false)
        {
            Debug.LogWarning($"잘못된 대사 인덱스입니다: {dialogueIndex}");
            return;
        }

        index = dialogueIndex;
    }

    public void PrepareDialogue()
    {
        index = 0;
        activeDialogues = GetDialoguesByQuestState();
    }

    private DialogueLine[] GetDialoguesByQuestState()
    {
        if (string.IsNullOrEmpty(relatedQuestId))
        {
            return dialogues;
        }

        if (QuestManager.Instance == null)
        {
            Debug.LogWarning("QuestManager가 씬에 없습니다. 기본 대사를 사용합니다.");
            return dialogues;
        }

        QuestState state = QuestManager.Instance.GetQuestState(relatedQuestId);

        switch (state)
        {
            case QuestState.InProgress:
                if (inProgressDialogues != null && inProgressDialogues.Length > 0)
                    return inProgressDialogues;
                break;
            case QuestState.Completed:
                if (completedDialogues != null && completedDialogues.Length > 0)
                    return completedDialogues;
                break;
            case QuestState.Rewarded:
                if(rewardedDialogues != null && rewardedDialogues.Length > 0)
                    return rewardedDialogues;
                break;
        }

        return dialogues;
    }
}