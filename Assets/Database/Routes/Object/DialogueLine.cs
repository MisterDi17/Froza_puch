using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    [TextArea(2, 4)]
    [SerializeField]
    [Tooltip("Максимум 94 символа")]
    private string text;

    public string Text => text.Length > 94 ? text.Substring(0, 94) : text;
}
