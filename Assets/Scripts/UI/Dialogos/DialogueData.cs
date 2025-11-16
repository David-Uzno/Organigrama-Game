using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "ScriptablesObjects/DialogueData", order = 1)]
public class DialogueData : ScriptableObject
{
    [System.Serializable]
    public class DialogueLine
    {
        public string SpeakerName;
        [TextArea(2, 5)] public string Line;
        public AudioClip audioClip;
    }

    public List<DialogueLine> dialogueLines;
}
