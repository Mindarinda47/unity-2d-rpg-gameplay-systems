using UnityEngine;

[System.Serializable]
public class DialogueChoice
{
    public string choiceText;
    public int nextLineIndex;

    [Header("Quest Action")]
    public string startQuestId;
    public string rewardQuestId;
}
