using UnityEngine;
using System.Collections;

public class NPCDialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueData _dialogueData;
    [SerializeField] private float triggerCooldown = 0.45f;

    private bool _playerInRange = false;
    private bool _canTrigger = true;

    private void Update()
    {
        if (_playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (DialogueManager.Instance == null) return;

            if (!DialogueManager.Instance.IsDialogueActive())
            {
                DialogueManager.Instance.StartDialogue(_dialogueData);
                StartCoroutine(TemporaryDisableTrigger());
            }
        }
    }
    private IEnumerator TemporaryDisableTrigger()
    {
        _canTrigger = false;
        yield return new WaitForSeconds(triggerCooldown);
        _canTrigger = true;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = true;
            Debug.Log("Presiona E para hablar");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = false;
        }
    }
}
