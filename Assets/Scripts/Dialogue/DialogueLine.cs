using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string speakerName;

    [TextArea(2, 4)]
    public string text;

    public Sprite portrait;

    public float typingSpeed = 0.03f;

    public DialogueChoice[] choices;

    public bool endDialogueAfterThisLine;
}
