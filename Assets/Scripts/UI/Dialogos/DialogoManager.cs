using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Collections;
public class DialogoManager : MonoBehaviour
{

    [System.Serializable]
    public class DialogueLine
    {
        public string speakerName;
        [TextArea(2, 5)] public string line;
    }

    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI dialogueText;
    public GameObject dialogueBox;

    public List<DialogueLine> dialogueLines;
    private int currentLine = 0;
    private bool isDialogueActive = false;
    private bool isTyping = false;

    void Start()
    {
        dialogueBox.SetActive(false);
    }

    void Update()
    {
        // Si hay un diálogo activo, presionar E o Space pasa al siguiente texto
        if (isDialogueActive && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space)))
        {
            if (!isTyping)
                NextLine();
        }
    }

    public void StartDialogue()
    {
        if (isDialogueActive) return; // Evita reiniciar si ya está abierto
        isDialogueActive = true;
        currentLine = 0;
        dialogueBox.SetActive(true);
        ShowLine();
    }

    void ShowLine()
    {
        if (currentLine < dialogueLines.Count)
        {
            speakerText.text = dialogueLines[currentLine].speakerName;
            StopAllCoroutines();
            StartCoroutine(TypeLine(dialogueLines[currentLine].line));
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char c in line.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.02f);
        }
        isTyping = false;
    }

    void NextLine()
    {
        currentLine++;
        ShowLine();
    }

    void EndDialogue()
    {
        dialogueBox.SetActive(false);
        isDialogueActive = false;
    }
}

