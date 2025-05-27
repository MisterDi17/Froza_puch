using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogues/Dialogue")]
public class DialogueData : ScriptableObject
{
    [SerializeField] private string dialogueName;
    [SerializeField] private List<DialogueLine> lines;

    public string DialogueName => dialogueName;
    public List<DialogueLine> Lines => lines;
}
