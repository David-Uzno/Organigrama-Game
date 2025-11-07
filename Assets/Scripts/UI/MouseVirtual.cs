using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseVirtual : MonoBehaviour
{
    #region Variables
    [SerializeField] PlayerInput _playerInput;
    [SerializeField] RectTransform _virtualCursor;
    [SerializeField] float _cursorSpeed = 100f;
    [SerializeField] bool _useMouseControl = true;
    [SerializeField] bool _useRightStickControl = true;
    Vector2 _virtualCursorPosition;
    Camera _mainCamera;
    #endregion

    #region Unity Methods
    void Start()
    {
        _mainCamera = Camera.main;
        // Centro de la pantalla
        _virtualCursorPosition = new Vector2(Screen.width / 2, Screen.height / 2);

        if (_virtualCursor == null)
        {
            Debug.LogError("¡El cursor virtual no está asignado en el Inspector!");
        }
    }

    void Update()
    {
        UpdateCursor();
    }

    private void UpdateCursor()
    {
        if (_useMouseControl)
        {
            // Obtener la posición del mouse
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            _virtualCursorPosition = mousePosition;
        }

        if (_useRightStickControl)
        {
            // Obtener la posición del stick derecho
            Vector2 stickInput = _playerInput.actions["Aim"].ReadValue<Vector2>();

            if (stickInput != Vector2.zero)
            {
                _virtualCursorPosition += stickInput * _cursorSpeed * Time.deltaTime;
                _virtualCursorPosition.x = Mathf.Clamp(_virtualCursorPosition.x, 0, Screen.width);
                _virtualCursorPosition.y = Mathf.Clamp(_virtualCursorPosition.y, 0, Screen.height);
            }
        }

        if (_virtualCursor != null)
        {
            _virtualCursor.position = _virtualCursorPosition;
        }
    }
    #endregion
}