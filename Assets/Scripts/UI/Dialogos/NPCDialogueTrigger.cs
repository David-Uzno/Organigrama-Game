using UnityEngine;
using System.Collections;

public class NPCDialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueData _dialogueData;

    private bool _playerInRange = false;
    private bool _dialogueCompleted = false; // Nuevo flag

    private void Update()
    {
        if (_playerInRange && !_dialogueCompleted && Input.GetKeyDown(KeyCode.E))
        {
            if (DialogueManager.Instance == null) return;

            if (!DialogueManager.Instance.IsDialogueActive())
            {
                DialogueManager.Instance.StartDialogue(_dialogueData, this);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = true;
            _dialogueCompleted = false; // Permite volver a hablar solo si el jugador sale y entra
            Debug.Log("Presiona E para hablar");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = false;
            _dialogueCompleted = false; // Resetea el flag al salir
        }
    }

    // Nuevo método para marcar el diálogo como completado
    public void MarkDialogueCompleted()
    {
        _dialogueCompleted = true;
    }
}
