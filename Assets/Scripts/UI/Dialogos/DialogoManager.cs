using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogoManager : MonoBehaviour
{
    #region Variables
    [System.Serializable]
    public class DialogueLine
    {
        public string SpeakerName;
        [TextArea(2, 5)] public string Line;
    }

    [Header("Dialogue Settings")]
    [SerializeField] private List<DialogueLine> _dialogueLines;
    [SerializeField] private WaitForSeconds _delayDuration = new(0.02f);

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI _speakerText;
    [SerializeField] private TextMeshProUGUI _dialogueText;
    [SerializeField] private GameObject _dialogueBox;

    private PlayerInput _playerInput;

    private int _currentLine = 0;
    private bool _isDialogueActive = false;
    private bool _isTyping = false;
    #endregion

    #region UnityMethods
    private void Start()
    {
        _dialogueBox.SetActive(false);
    }

    private void Update()
    {
        // Si hay un diálogo activo, presionar E o Space pasa al siguiente texto
        if (_isDialogueActive && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space)))
        {
            if (!_isTyping)
            {
                NextLine();
            }
        }
    }
    #endregion

    #region InternalDialogueLogic
    public void StartDialogue()
    {
        if (_isDialogueActive) return; // Evita reiniciar si ya está abierto
        _isDialogueActive = true;
        _currentLine = 0;
        _dialogueBox.SetActive(true);

        // Buscar PlayerInput si aún no está referenciado
        if (_playerInput == null)
        {
            _playerInput = FindFirstObjectByType<PlayerInput>();
        }

        if (_playerInput != null)
            _playerInput.enabled = false; // Desactiva el InputSystem

        ShowLine();
    }
    
    private void ShowLine()
    {
        if (_currentLine < _dialogueLines.Count)
        {
            _speakerText.text = _dialogueLines[_currentLine].SpeakerName;
            StopAllCoroutines();
            StartCoroutine(TypeLine(_dialogueLines[_currentLine].Line));
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeLine(string line)
    {
        _isTyping = true;
        _dialogueText.text = "";
        foreach (char c in line.ToCharArray())
        {
            _dialogueText.text += c;
            yield return _delayDuration;
        }
        _isTyping = false;
    }

    private void NextLine()
    {
        _currentLine++;
        ShowLine();
    }

    private void EndDialogue()
    {
        _dialogueBox.SetActive(false);
        _isDialogueActive = false;

        // Buscar PlayerInput si aún no está referenciado
        if (_playerInput == null)
        {
            _playerInput = FindFirstObjectByType<PlayerInput>();
            if (_playerInput != null)
                Debug.Log("[DialogoManager] PlayerInput found at EndDialogue: " + _playerInput.gameObject.name);
            else
                Debug.LogWarning("[DialogoManager] PlayerInput not found at EndDialogue.");
        }

        if (_playerInput != null)
            _playerInput.enabled = true; // Reactiva el InputSystem
    }
    #endregion
}
