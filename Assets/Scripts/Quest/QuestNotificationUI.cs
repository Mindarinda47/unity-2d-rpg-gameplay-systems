using System.Collections;
using TMPro;
using UnityEngine;

public class QuestNotificationUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private CanvasGroup notificationCanvasGroup;
    [SerializeField] private TextMeshProUGUI notificationText;
    [SerializeField] private RectTransform notificationRectTransform;

    [Header("Setting")]
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private float displayDuration = 2f;
    [SerializeField] private Vector2 notificationPosition;
    [SerializeField] private float moveDistance = 30f;

    private QuestManager questManager;
    private Coroutine notificationCoroutine;

    private void Start()
    {
        if (notificationCanvasGroup == null ||
            notificationText == null ||
            notificationRectTransform == null)
        {
            Debug.LogWarning("Quest notification UI references are missing.");
            return;
        }

        questManager = QuestManager.Instance;

        if (questManager == null)
        {
            Debug.LogWarning("QuestManger가 씬에 없습니다.");
            return;
        }

        questManager.QuestStateChanged += ShowQuestNotification;

        notificationRectTransform.anchoredPosition = notificationPosition;

        HideImmediately();
    }

    private void OnDestroy()
    {
        if (questManager != null)
        {
            questManager.QuestStateChanged -= ShowQuestNotification;
        }
    }

    private void ShowQuestNotification(QuestData quest, QuestState state)
    {
        if (quest == null)
            return;

        string message = CreateNotificationMessage(quest, state);

        if (string.IsNullOrEmpty(message))
            return;

        if (notificationCoroutine != null)
        {
            StopCoroutine(notificationCoroutine);
        }

        notificationCoroutine = StartCoroutine(
            ShowNotificationCoroutine(message)
        );
    }

    private string CreateNotificationMessage(
        QuestData quest,
        QuestState state
    )
    {
        switch (state)
        {
            case QuestState.InProgress:
                return $"퀘스트 시작\n{quest.QuestName}";

            case QuestState.Completed:
                return $"퀘스트 완료\n{quest.QuestName}";

            case QuestState.Rewarded:
                return $"보상 수령\n{quest.QuestName}";

            default:
                return "";
        }
    }

    private IEnumerator ShowNotificationCoroutine(string message)
    {
        if (notificationCanvasGroup == null ||
            notificationText == null ||
            notificationRectTransform == null)
        {
            Debug.LogWarning("퀘스트 알림 UI 연결이 누락되었습니다.");
            notificationCoroutine = null;
            yield break;
        }

        notificationText.text = message;

        // 페이드 인
        yield return AnimateNotification(0f, 1f, notificationPosition - Vector2.up * moveDistance, notificationPosition);

        // 완전히 보이는 상태로 유지
        yield return new WaitForSecondsRealtime(displayDuration);

        // 페이드 아웃
        yield return AnimateNotification(1f, 0f, notificationPosition, notificationPosition - Vector2.up * moveDistance);

        notificationCoroutine = null;
    }

    private IEnumerator AnimateNotification(float startAlpha, float targetAlpha, Vector2 startPosition, Vector2 targetPosition)
    {
        if (notificationCanvasGroup == null ||
            notificationRectTransform == null)
        {
            Debug.LogWarning("퀘스트 알림 애니메이션 UI 연결이 누락되었습니다.");
            yield break;
        }

        if (fadeDuration <= 0f)
        {
            notificationCanvasGroup.alpha = targetAlpha;
            notificationRectTransform.anchoredPosition = targetPosition;
            yield break;
        }

        float elapsedTime = 0f;
        notificationCanvasGroup.alpha = startAlpha;
        notificationRectTransform.anchoredPosition = startPosition;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime; // 게임 일시정지에 영향 받지 않는 시간. UI 애니메이션에 사용됨.

            float progress = elapsedTime / fadeDuration;

            notificationCanvasGroup.alpha = Mathf.Lerp(
                startAlpha,
                targetAlpha,
                progress
            );

            notificationRectTransform.anchoredPosition = Vector2.Lerp(
                startPosition,
                targetPosition,
                progress
            );

            yield return null;
        }

        notificationCanvasGroup.alpha = targetAlpha; // 최종값 보정, 프레임 시간이 정확히 일정하지 않을 수도 있음. 그래서 마지막에 보정.
        notificationRectTransform.anchoredPosition = targetPosition;
    }

    private void HideImmediately()
    {
        if (notificationCanvasGroup != null)
        {
            notificationCanvasGroup.alpha = 0f;
        }
    }
}
