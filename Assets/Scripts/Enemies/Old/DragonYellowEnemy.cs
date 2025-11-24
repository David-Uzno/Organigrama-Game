using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonYellowEnemy : FatherEnemy
{
    #region Variables
    [Header("Movement")]
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _changeDirectionTime = 3f;
    private Vector2 _moveDirection;
    private float _timeToChangeDirection;

    [Header("Limits")]
    [SerializeField] private float _boundaryBuffer = 0.5f;
    private Vector2 _cameraBoundsMin;
    private Vector2 _cameraBoundsMax;
    private Camera _mainCamera;
    #endregion

    #region Unity Methods
    private void Start()
    {
        InitializeComponents();
        InitializeMovement();
    }

    private void Update()
    {
        Move();
        ChangeDirectionIfNeeded();
        CheckBoundaries();
    }
    #endregion

    #region Initialization
    private void InitializeComponents()
    {
        _mainCamera = Camera.main;
    }

    private void InitializeMovement()
    {
        _timeToChangeDirection = Time.time + _changeDirectionTime;
        GenerateRandomDirection();
        UpdateCameraBounds();
    }

    private void UpdateCameraBounds()
    {
        if (_mainCamera != null)
        {
            _cameraBoundsMin = _mainCamera.ViewportToWorldPoint(new Vector3(0, 0, _mainCamera.transform.position.z));
            _cameraBoundsMax = _mainCamera.ViewportToWorldPoint(new Vector3(1, 1, _mainCamera.transform.position.z));
        }
    }
    #endregion

    #region Movement
    private void Move()
    {
        _rb.linearVelocity = _moveDirection * _moveSpeed;
    }

    private void ChangeDirectionIfNeeded()
    {
        if (Time.time >= _timeToChangeDirection)
        {
            GenerateRandomDirection();
            _timeToChangeDirection = Time.time + _changeDirectionTime;
        }
    }

    private void GenerateRandomDirection()
    {
        float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        _moveDirection = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle));
    }

    private void CheckBoundaries()
    {
        if (_mainCamera != null)
        {
            Vector3 pos = transform.position;

             // Movimiento Derecha
            if (pos.x < _cameraBoundsMin.x + _boundaryBuffer)
            {
                _moveDirection.x = Mathf.Abs(_moveDirection.x);
            }
            // Movimiento Izquierda
            else if (pos.x > _cameraBoundsMax.x - _boundaryBuffer)
            {
                _moveDirection.x = -Mathf.Abs(_moveDirection.x);
            }

            // Movimiento Arriba
            if (pos.y < _cameraBoundsMin.y + _boundaryBuffer)
            {
                _moveDirection.y = Mathf.Abs(_moveDirection.y);
            }
            // Movimiento Abajo
            else if (pos.y > _cameraBoundsMax.y - _boundaryBuffer)
            {
                _moveDirection.y = -Mathf.Abs(_moveDirection.y);
            }
        }
    }
    #endregion
}