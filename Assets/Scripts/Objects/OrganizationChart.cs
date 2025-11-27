using UnityEngine;
using UnityEngine.InputSystem;

public class OrganizationChart : MonoBehaviour
{
    [SerializeField] private GameObject _targetObject;
    [SerializeField] private bool _pauseTimeOnShow = true;
    private string _playerTag = "Player";
    private bool _isPlayerInside = false;
    private PlayerInput _playerInput;
    private InputAction _actionInput;

    private void Awake()
    {
        _playerInput = FindFirstObjectByType<PlayerInput>();
        if (_playerInput != null)
        {
            _actionInput = _playerInput.actions["Action"];
        }
    }

    private void OnEnable()
    {
        if (_actionInput != null)
            _actionInput.performed += OnActionPerformed;
    }

    private void OnDisable()
    {
        if (_actionInput != null)
            _actionInput.performed -= OnActionPerformed;
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.CompareTag(_playerTag))
        {
            _isPlayerInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag(_playerTag))
        {
            _isPlayerInside = false;
        }
    }

    private void OnActionPerformed(InputAction.CallbackContext context)
    {
        if (_isPlayerInside && _targetObject != null)
        {
            bool willShow = !_targetObject.activeSelf;
            _targetObject.SetActive(willShow);

            if (_pauseTimeOnShow)
            {
                if (willShow)
                {
                    Time.timeScale = 0f;
                }
                else
                {
                    Time.timeScale = 1f;
                }
            }
        }
    }
}
