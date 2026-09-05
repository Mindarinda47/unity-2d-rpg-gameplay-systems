using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager: MonoBehaviour
{
    [SerializeField] private GameObject dialogueUI;

    [Header("Speaker")]
    [SerializeField] private GameObject speakerNameBox;
    [SerializeField] private TextMeshProUGUI speakerNameText;

    [Header("Portrait")]
    [SerializeField] private GameObject portraitBox;
    [SerializeField] private Image portraitImage;

    [Header("Dialogue")]
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Choice")]
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private Transform choiceButtonParent;
    [SerializeField] private Button choiceButtonPrefab;

    private NPCInteractable currentNPC;
    private DialogueLine currentLine;
    private Coroutine typingCoroutine;
    private readonly List<Button> spawnedChoiceButtons = new List<Button>();

    public bool IsOpen { get; private set; }
    public bool IsTyping { get; private set; }

    public void Open(NPCInteractable npc)
    {
        currentNPC = npc;
        currentNPC.PrepareDialogue();

        IsOpen = true;
        dialogueUI.SetActive(true);

        ShowNextLine();
    }

    public bool NextDialogue()
    {
        if (currentNPC == null)
            return false;

        if (IsTyping)
        {
            CompleteCurrentLine();
            return true;
        }

        if (HasCurrentChoices())
        {
            return true;
        }

        if (currentLine != null && currentLine.endDialogueAfterThisLine)
        {
            EndDialogue();
            return false;
        }

        if (currentNPC.IsDialogueEnd())
        {
            EndDialogue();
            return false;
        }

        ShowNextLine();

        return true;
    }

    public void Close()
    {
        StopTyping();
        ClearChoices();

        IsOpen=false;
        dialogueUI.SetActive(false);

        ClearLineUI();

        currentLine = null;
    }

    public void ShowNextLine()
    {
        ClearChoices();

        currentLine = currentNPC.GetNextDialogue();

        if (currentLine == null)
        {
            ClearLineUI();
            return;
        }

        ShowSpeakerName(currentLine);
        ShowPortrait(currentLine);

        StopTyping();
        typingCoroutine = StartCoroutine(TypeLine(currentLine));
    }

    private void ShowSpeakerName(DialogueLine line)
    {
        bool hasSpeakerName = !string.IsNullOrEmpty(line.speakerName);

        if (speakerNameBox != null)
        {
            speakerNameBox.SetActive(hasSpeakerName);
        }

        if (speakerNameText != null)
        {
            speakerNameText.text = hasSpeakerName ? line.speakerName : "";
        }
    }

    private void ShowPortrait(DialogueLine line)
    {
        bool hasPortrait = line.portrait != null;

        if (portraitBox != null)
        {
            portraitBox.SetActive(hasPortrait);
        }

        if (portraitImage != null)
        {
            portraitImage.sprite = line.portrait;
            portraitImage.preserveAspect = true;
            portraitImage.enabled = hasPortrait;
        }
    }

    private IEnumerator TypeLine(DialogueLine line)
    {
        IsTyping = true;
        dialogueText.text = "";

        string text = line.text;
        float typingSpeed = line.typingSpeed;

        if (string.IsNullOrEmpty(text))
        {
            IsTyping = false;
            ShowChoicesIfNeeded(line);
            yield break;
        }

        if (typingSpeed <= 0f)
        {
            dialogueText.text = text;
            IsTyping = false;
            ShowChoicesIfNeeded(line);
            yield break;
        }

        foreach (char letter in text)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        IsTyping = false;
        typingCoroutine = null;

        ShowChoicesIfNeeded(line);
    }

    private void CompleteCurrentLine()
    {
        if (currentLine == null)
            return;

        StopTyping();

        dialogueText.text = currentLine.text;
        IsTyping = false;

        ShowChoicesIfNeeded(currentLine);
    }

    private void StopTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        IsTyping = false;
    }

    private void ClearLineUI()
    {
        if (speakerNameBox != null)
        {
            speakerNameBox.SetActive(false);
        }

        if (speakerNameText != null)
        {
            speakerNameText.text = "";
        }

        if (dialogueText != null)
        {
            dialogueText.text = "";
        }

        if (portraitImage != null)
        {
            portraitImage.sprite = null;
            portraitImage.enabled = false;
        }

        if (portraitBox != null)
        {
            portraitBox.SetActive(false);
        }
    }

    private bool HasCurrentChoices()
    {
        return currentLine != null &&
               currentLine.choices != null &&
               currentLine.choices.Length > 0;
    }

    private void ShowChoicesIfNeeded(DialogueLine line)
    {
        if (line.choices == null || line.choices.Length == 0)
            return;

        if (choicePanel == null || choiceButtonParent == null || choiceButtonPrefab == null)
        {
            Debug.LogWarning("선택지 UI 연결이 누락되었습니다.");
        }

        choicePanel.SetActive(true);

        foreach (DialogueChoice choice in line.choices)
        {
            Button button = Instantiate(choiceButtonPrefab, choiceButtonParent);
            button.gameObject.SetActive(true);

            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();

            if (buttonText != null)
            {
                buttonText.text = choice.choiceText;
            }

            DialogueChoice selectedChoice = choice; // 버튼 클릭 이벤트는 지금 바로 실행되는 게 아닌 나중에 클릭했을 때 실행되므로 안전하게 현재 선택지의 이동 번호를 지역 변수에 복사해두는 것.

            // 이렇게 해두면 이 버튼은 자기만의 이동 번호를 기억함.
            button.onClick.AddListener(() =>
            {
                OnChoiceSelected(selectedChoice);
            });

            spawnedChoiceButtons.Add(button);
        }
    }

    private void ClearChoices()
    {
        foreach (Button button in spawnedChoiceButtons)
        {
            if (button != null)
            {
                Destroy(button.gameObject);
            }
        }

        spawnedChoiceButtons.Clear();

        if (choicePanel != null)
        {
            choicePanel.SetActive(false);
        }
    }

    private void OnChoiceSelected(DialogueChoice choice)
    {
        if (currentNPC == null)
            return;

        if (choice == null)
            return;

        SetChoiceButtonsInteractable(false);

        if (currentNPC.IsValidDialogueIndex(choice.nextLineIndex) == false)
        {
            Debug.LogWarning($"선택지의 이동 대상이 잘못되었습니다: {choice}");
            SetChoiceButtonsInteractable(true);
            return;
        }

        ApplyQuestActions(choice);

        currentNPC.SetDialogueIndex(choice.nextLineIndex);
        ShowNextLine();
    }

    private void EndDialogue()
    {
        if (currentNPC != null)
        {
            currentNPC.EndInteract();
            currentNPC = null;
        }

        Close();
    }

    // 선택지 버튼 빠르게 여러 번 클릭 시 생기는 버그 방지
    private void SetChoiceButtonsInteractable(bool value)
    {
        foreach (Button button in spawnedChoiceButtons)
        {
            if (button != null)
            {
                button.interactable = value;
            }
        }
    }

    private void ApplyQuestActions(DialogueChoice choice)
    {
        if (choice == null)
            return;

        if (QuestManager.Instance == null)
        {
            Debug.LogWarning("QuestManager가 씬에 없습니다.");
            return;
        }

        if (string.IsNullOrEmpty(choice.startQuestId) == false)
        {
            QuestManager.Instance.StartQuest(choice.startQuestId);
        }

        if (string.IsNullOrEmpty(choice.rewardQuestId) == false)
        {
            QuestManager.Instance.RewardQuest(choice.rewardQuestId);
        }
    }
}