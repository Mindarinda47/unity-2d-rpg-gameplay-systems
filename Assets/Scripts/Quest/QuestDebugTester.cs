using UnityEngine;

public class QuestDebugTester : MonoBehaviour
{
    [SerializeField] private string questId = "herb_quest";

    #if UNITY_EDITOR
    private void Update()
    {
        if (QuestManager.Instance == null)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            QuestManager.Instance.StartQuest(questId);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            QuestManager.Instance.CompleteQuest(questId);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            QuestManager.Instance.RewardQuest(questId);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            QuestState state = QuestManager.Instance.GetQuestState(questId);
            Debug.Log($"현재 퀘스트 상태: {state}");
        }
    }
    #endif
}