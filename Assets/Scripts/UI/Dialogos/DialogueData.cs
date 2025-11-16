using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "ScriptablesObjects/DialogueData", order = 1)]
public class DialogueData : ScriptableObject
{
    [System.Serializable]
    public class DialogueAudio
    {
        public AudioClip clip;
        public bool dontInterruptOnNext = false;
    }

    [System.Serializable]
    public class DialogueLine
    {
        public string SpeakerName;
        [TextArea(2, 5)] public string Line;
        public List<DialogueAudio> audioClips;
    }

    public List<DialogueLine> dialogueLines;
}
