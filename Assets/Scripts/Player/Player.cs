using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, IDamageable, IHealable
{
    #region Variables
    [Header("Movement")]
    [SerializeField] PlayerInput _playerInput;
    [SerializeField] private Rigidbody2D _RB;
    [SerializeField] private float _speed = 5f;
    
    [Header("Life")]
    [SerializeField] private int _life = 3;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Color _damageFlashColor = Color.red;
    [SerializeField] private int _damageFlashCount = 3;
    [SerializeField] private float _damageFlashDuration = 0.75f;
    private Coroutine _flashCoroutine;
    public Color _originalColor;
    private Color _currentColor;

    [Header("Other Components")]
    [SerializeField] private Animator _animator;

    private bool _isInvincible = false;
    private bool _isFlashing = false;
    #endregion

    #region Unity Methods
    private void Start()
    {
        if (_spriteRenderer == null)
        {
            Debug.LogError("¡SpriteRenderer no está asignado!");
            enabled = false;
            return;
        }

        _originalColor = _spriteRenderer.color;
        _currentColor = _originalColor;

        if (_animator != null)
        {
            _animator.SetBool("Walk", false);
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            TakeDamage(1);
        }
    }
    #endregion

    #region Movement
    private void HandleMovement()
    {
        Vector2 movementInput = _playerInput.actions["Movement"].ReadValue<Vector2>();

        _RB.linearVelocity = movementInput * _speed;
        HandleRotation(movementInput.x);

        if (_animator != null)
        {
            UpdateAnimations(movementInput);
        }
    }

    private void HandleRotation(float movementHorizontal)
    {
        if (movementHorizontal < 0)
        {
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        else if (movementHorizontal > 0)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }
    #endregion

    #region Animation
    private void UpdateAnimations(Vector2 movementInput)
    {
        bool isWalking = movementInput != Vector2.zero;
        _animator.SetBool("Walk", isWalking);
    }
    #endregion

    #region Life
    public void RecoverLife(int amount)
    {
        int maxLife = GameManager.Instance.GetMaxLife();

        if (_life < maxLife)
        {
            _life = Mathf.Min(_life + amount, maxLife);
            GameManager.Instance.RecoverLife(amount);
        }
    }

    public void TakeDamage(float damage)
    {
        if (_isInvincible) return;

        _life -= (int)damage;
        if (_flashCoroutine != null)
        {
            StopCoroutine(_flashCoroutine);
        }
        GameManager.Instance.LoseLife();

        _flashCoroutine = StartCoroutine(FlashSpriteDamage());

        if (_life <= 0)
        {
            SceneManager.LoadScene("GameOver");
        }
    }

    private IEnumerator FlashSpriteDamage()
    {
        _isFlashing = true;
        for (int i = 0; i < _damageFlashCount; i++)
        {
            if (!_isInvincible)
            {
                SetPlayerColor(_damageFlashColor);
            }
            yield return new WaitForSeconds(_damageFlashDuration / 2);

            if (!_isInvincible)
            {
                SetPlayerColor(_originalColor);
            }
            yield return new WaitForSeconds(_damageFlashDuration / 2);
        }
        _isFlashing = false;
        if (!_isInvincible)
        {
            SetPlayerColor(_originalColor);
        }
    }

    private void SetPlayerColor(Color color)
    {
        _spriteRenderer.color = color;
    }

    public void SetInvincibility(bool isInvincible, Color invincibleColor)
    {
        _isInvincible = isInvincible;

        if (isInvincible)
        {
            SetPlayerColor(invincibleColor);
        }
        else
        {
            if (_isFlashing)
            {
                SetPlayerColor(_damageFlashColor);
            }
            else
            {
                SetPlayerColor(_originalColor);
            }
        }
    }
    #endregion
}