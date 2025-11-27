using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class OrganizationChart : MonoBehaviour
{
    private static WaitForSeconds _waitForSeconds0_5 = new WaitForSeconds(0.5f);
    [SerializeField] private GameObject _targetObject;
    [SerializeField] private bool _pauseTimeOnShow = true;
    private string _playerTag = "Player";
    private bool _isPlayerInside = false;
    private PlayerInput _playerInput;
    private InputAction _actionInput;

    private void Start()
    {
        StartCoroutine(FindPlayerInputDelayed());
    }

    private IEnumerator FindPlayerInputDelayed()
    {
        yield return _waitForSeconds0_5;
        _playerInput = FindFirstObjectByType<PlayerInput>();
        if (_playerInput != null)
        {
            _actionInput = _playerInput.actions["Action"];
            if (_actionInput != null)
                _actionInput.performed += OnActionPerformed;
        }
    }

    private void OnDisable()
    {
        if (_actionInput != null)
            _actionInput.performed -= OnActionPerformed;
    }

    private void OnTriggerEnter2D(Collider2D collider)
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
