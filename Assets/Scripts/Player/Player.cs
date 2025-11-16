using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, IDamageable, IHealable
{
    [Header("Movement")]
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private float _speed = 5f;

    [Header("Life")]
    [SerializeField] private int _life = 3;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Color _damageFlashColor = Color.red;
    [SerializeField] private int _damageFlashCount = 3;
    [SerializeField] private float _damageFlashDuration = 0.75f;

    private Coroutine _flashCoroutine;
    [HideInInspector] public Color _originalColor;
    private bool _isInvincible;
    private bool _isFlashing;

    [Header("Other Components")]
    [SerializeField] private Animator _animator;

    [Header("Layers")]
    [SerializeField] private string _enemyLayerName = "Enemy";
    [SerializeField] private string _weaponLayerName = "PlayerWeapon";

    private int _enemyLayer = -1;
    private int _weaponLayer = -1;

    private void Awake()
    {
        ValidateComponents();
        InitializeColors();
        CacheLayers();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;
        if (collision.GetComponentInParent<MeleeHit>() != null) return;
        if (collision.transform.IsChildOf(transform)) return;
        if (_weaponLayer != -1 && collision.gameObject.layer == _weaponLayer) return;

        if (_enemyLayer != -1)
        {
            Transform t = collision.transform;
            while (t != null)
            {
                if (t.gameObject.layer == _enemyLayer)
                {
                    TakeDamage(1);
                    return;
                }
                t = t.parent;
            }
        }

        if (collision.GetComponentInParent<FatherEnemy>() != null)
        {
            TakeDamage(1);
        }
    }

    private void HandleMovement()
    {
        Vector2 movementInput = _playerInput.actions["Movement"].ReadValue<Vector2>();
        _rigidbody.linearVelocity = movementInput * _speed;
        HandleRotation(movementInput.x);

        if (_animator != null)
            _animator.SetBool("Walk", movementInput != Vector2.zero);
    }

    private void HandleRotation(float movementHorizontal)
    {
        if (movementHorizontal < 0)
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        else if (movementHorizontal > 0)
            transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    public void RecoverLife(int amount)
    {
        int maxLife = GameManager.Instance.GetMaxLife();
        if (_life < maxLife)
        {
            int recoverAmount = Mathf.Min(amount, maxLife - _life);
            _life += recoverAmount;
            GameManager.Instance.RecoverLife(recoverAmount);
        }
    }

    public void TakeDamage(float damage)
    {
        if (_isInvincible) return;

        _life -= (int)damage;
        if (_flashCoroutine != null)
            StopCoroutine(_flashCoroutine);

        GameManager.Instance.LoseLife();
        _flashCoroutine = StartCoroutine(FlashSpriteDamage());

        if (_life <= 0)
            SceneManager.LoadScene("GameOver");
    }

    private IEnumerator FlashSpriteDamage()
    {
        _isFlashing = true;
        for (int i = 0; i < _damageFlashCount; i++)
        {
            if (!_isInvincible)
                SetPlayerColor(_damageFlashColor);
            yield return new WaitForSeconds(_damageFlashDuration / 2);

            if (!_isInvincible)
                SetPlayerColor(_originalColor);
            yield return new WaitForSeconds(_damageFlashDuration / 2);
        }
        _isFlashing = false;
        if (!_isInvincible)
            SetPlayerColor(_originalColor);
    }

    private void SetPlayerColor(Color color)
    {
        _spriteRenderer.color = color;
    }

    public void SetInvincibility(bool isInvincible, Color invincibleColor)
    {
        _isInvincible = isInvincible;
        if (isInvincible)
            SetPlayerColor(invincibleColor);
        else
            SetPlayerColor(_isFlashing ? _damageFlashColor : _originalColor);
    }

    private void ValidateComponents()
    {
        if (_spriteRenderer == null)
        {
            Debug.LogError("¡SpriteRenderer no está asignado!");
            enabled = false;
        }
        if (_rigidbody == null)
        {
            Debug.LogError("¡Rigidbody2D no está asignado!");
            enabled = false;
        }
        if (_playerInput == null)
        {
            Debug.LogError("¡PlayerInput no está asignado!");
            enabled = false;
        }
    }

    private void InitializeColors()
    {
        if (_spriteRenderer != null)
            _originalColor = _spriteRenderer.color;
    }

    private void CacheLayers()
    {
        _enemyLayer = LayerMask.NameToLayer(_enemyLayerName);
        _weaponLayer = LayerMask.NameToLayer(_weaponLayerName);

        if (_enemyLayer == -1)
            Debug.LogWarning($"Player: la layer '{_enemyLayerName}' no existe. Revisa __Project Settings > Tags and Layers__.");
        if (_weaponLayer == -1)
            Debug.LogWarning($"Player: la layer '{_weaponLayerName}' no existe. Revisa __Project Settings > Tags and Layers__.");
    }
}
