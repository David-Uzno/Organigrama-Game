using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections;

public class OrganizationChart : MonoBehaviour
{
    [Header("Refences")]
    [SerializeField] private RectTransform _targetRectTransform;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private bool _pauseTimeOnShow = true;
    private static readonly WaitForSeconds s_waitForSeconds0_5 = new(0.5f);
    private string _playerTag = "Player";
    private bool _isPlayerInside = false;
    private PlayerInput _playerInput;
    private InputAction _actionInput;

    [Header("Navigation")]
    [SerializeField] private float _minZoom = 1f;
    [SerializeField] private float _maxZoom = 2.5f;
    [SerializeField] private float _zoomSpeed = 5f;
    [SerializeField] private float _dragSpeed = 1f;
    private bool _isDragging = false;
    private Vector2 _lastMousePosition;
    private float _zoom = 1f;


    private void Start()
    {
        StartCoroutine(FindPlayerInputDelayed());
        if (_targetRectTransform != null)
            _targetRectTransform.gameObject.SetActive(false);
    }

    private IEnumerator FindPlayerInputDelayed()
    {
        yield return s_waitForSeconds0_5;
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
        if (_isPlayerInside && _targetRectTransform != null)
        {
            bool willShow = !_targetRectTransform.gameObject.activeSelf;
            _targetRectTransform.gameObject.SetActive(willShow);

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

    private void Update()
    {
        if (_targetRectTransform != null && _targetRectTransform.gameObject.activeSelf)
        {
            HandleMouseDrag();
            HandleMouseZoom();
        }
    }

    private void HandleMouseDrag()
    {
        // Detectar inicio del arrastre
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            _isDragging = true;
            _lastMousePosition = Mouse.current.position.ReadValue();
        }
        // Detectar fin del arrastre
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            _isDragging = false;
        }

        // Mientras se mantenga presionado el botón izquierdo, arrastrar
        if (_isDragging && Mouse.current.leftButton.isPressed)
        {
            Vector2 currentMousePosition = Mouse.current.position.ReadValue();
            Vector2 delta = (currentMousePosition - _lastMousePosition) * _dragSpeed / _canvas.scaleFactor;
            Vector2 newPosition = _targetRectTransform.anchoredPosition + delta;

            _targetRectTransform.anchoredPosition = ClampPositionToBounds(newPosition);
            _lastMousePosition = currentMousePosition;
        }
    }

    private void HandleMouseZoom()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;
        if (Mathf.Abs(scroll) > 0.01f)
        {
            float prevZoom = _zoom;
            _zoom = Mathf.Clamp(_zoom + scroll * _zoomSpeed * 0.01f, _minZoom, _maxZoom);
            Vector3 scale = Vector3.one * _zoom;
            _targetRectTransform.localScale = scale;

            // Ajustar la posición después de hacer zoom para mantener los límites
            _targetRectTransform.anchoredPosition = ClampPositionToBounds(_targetRectTransform.anchoredPosition);
        }
    }

    // Método auxiliar para limitar la posición
    private Vector2 ClampPositionToBounds(Vector2 position)
    {
        Vector2 canvasSize = _canvas.GetComponent<RectTransform>().rect.size;
        Vector2 imageSize = _targetRectTransform.rect.size * _targetRectTransform.localScale;

        float limitX = (imageSize.x - canvasSize.x) / 2f;
        if (limitX < 0) limitX = 0;
        float limitY = (imageSize.y - canvasSize.y) / 2f;
        if (limitY < 0) limitY = 0;

        position.x = Mathf.Clamp(position.x, -limitX, limitX);
        position.y = Mathf.Clamp(position.y, -limitY, limitY);

        return position;
    }
}
