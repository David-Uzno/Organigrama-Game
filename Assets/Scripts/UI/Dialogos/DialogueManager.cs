using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    #region Singleton
    public static DialogueManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region Variables
    [Header("Dialogue Settings")]
    [SerializeField] private WaitForSeconds _delayDuration = new(0.02f);

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI _speakerText;
    [SerializeField] private TextMeshProUGUI _dialogueText;
    [SerializeField] private GameObject _dialogueBox;

    private PlayerInput _playerInput;

    private int _currentLine = 0;
    private bool _isDialogueActive = false;
    private bool _isTyping = false;

    private DialogueData _dialogueData;

    private AudioSource _audioSource; // Nuevo AudioSource privado
    #endregion

    #region UnityMethods
    private void Start()
    {
        _dialogueBox.SetActive(false);
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.playOnAwake = false;
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
    public void StartDialogue(DialogueData dialogueData)
    {
        if (_isDialogueActive) return; // Evita reiniciar si ya está abierto
        _isDialogueActive = true;
        _currentLine = 0;
        _dialogueBox.SetActive(true);

        _dialogueData = dialogueData; // Asigna el DialogueData recibido

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
        if (_dialogueData != null && _currentLine < _dialogueData.dialogueLines.Count)
        {
            var lineData = _dialogueData.dialogueLines[_currentLine];
            _speakerText.text = lineData.SpeakerName;
            StopAllCoroutines();
            StartCoroutine(TypeLine(lineData.Line));

            // Reproduce el audio si está asignado
            if (lineData.audioClip != null)
            {
                _audioSource.Stop();
                _audioSource.clip = lineData.audioClip;
                _audioSource.Play();
            }
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

        if (_playerInput == null)
        {
            _playerInput = FindFirstObjectByType<PlayerInput>();
        }

        if (_playerInput != null)
        {
            _playerInput.enabled = true;
        }
    }
    #endregion
}
