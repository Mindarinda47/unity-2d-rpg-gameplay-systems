using TMPro;
using UnityEngine;

public class QuestTrackerUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject trackerPanel;
    [SerializeField] private TextMeshProUGUI questTitleText;
    [SerializeField] private TextMeshProUGUI questObjectiveText;

    private QuestManager questManager;

    private void Start()
    {
        questManager = QuestManager.Instance;

        if (questManager == null)
        {
            Debug.LogWarning("QuestManager가 씬에 없습니다.");
            HideTracker();
            return;
        }

        questManager.QuestStateChanged += HandleQuestStateChanged;
        questManager.QuestProgressChanged += HandleQuestProgressChanged;

        RefreshTracker();
    }

    private void OnDestroy()
    {
        if (questManager != null)
        {
            questManager.QuestStateChanged -= HandleQuestStateChanged;
            questManager.QuestProgressChanged -= HandleQuestProgressChanged;
        }
    }

    private void HandleQuestStateChanged(
        QuestData changedQuest,
        QuestState changedState)
    {
        RefreshTracker();
    }

    private void HandleQuestProgressChanged(
        QuestData changedQuest,
        int currentAmount,
        int requiredAmount)
    {
        RefreshTracker();
    }

    private void RefreshTracker()
    {
        if (trackerPanel == null ||
            questTitleText == null ||
            questObjectiveText == null)
        {
            Debug.LogWarning("퀘스트 추적 UI 연결이 누락되었습니다.");
            return;
        }

        if (questManager.TryGetTrackedQuest(out QuestData trackedQuest) == false)
        {
            HideTracker();
            return;
        }

        trackerPanel.SetActive(true);

        questTitleText.text = trackedQuest.QuestName;

        switch (trackedQuest.State)
        {
            case QuestState.InProgress:
                questObjectiveText.text =
                    $"{trackedQuest.ObjectiveDescription}\n" +
                    $"{trackedQuest.CurrentAmount} / " +
                    $"{trackedQuest.RequiredAmount}";
                break;
            case QuestState.Completed:
                questObjectiveText.text =
                    trackedQuest.CompletedDescription;
                break;

            default:
                HideTracker();
                break;
        }
    }

    private void HideTracker()
    {
        if (trackerPanel != null)
        {
            trackerPanel.SetActive(false);
        }
    }
}
