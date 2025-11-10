using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "Dialogues/DialogueData", order = 1)]
public class DialogueData : ScriptableObject
{
    [System.Serializable]
    public class DialogueLine
    {
        public string SpeakerName;
        [TextArea(2, 5)] public string Line;
    }

    public List<DialogueLine> dialogueLines;
}
