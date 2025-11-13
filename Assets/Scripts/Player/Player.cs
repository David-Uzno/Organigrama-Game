using System.Collections;
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

    [Header("Layers")]
    [SerializeField] private string enemyLayerName = "Enemy"; // nombre de la layer que usan los enemigos
    [SerializeField] private string weaponLayerName = "PlayerWeapon"; // layer del arma/hitbox

    private int enemyLayer = -1;
    private int weaponLayer = -1;

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

        enemyLayer = LayerMask.NameToLayer(enemyLayerName);
        weaponLayer = LayerMask.NameToLayer(weaponLayerName);

        if (enemyLayer == -1)
            Debug.LogWarning($"Player: la layer '{enemyLayerName}' no existe. Revisa __Project Settings > Tags and Layers__.");
        if (weaponLayer == -1)
            Debug.LogWarning($"Player: la layer '{weaponLayerName}' no existe. Revisa __Project Settings > Tags and Layers__.");
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    // Ahora usamos layers (y protección extra por componente) en lugar de tags
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;

        // Ignorar cualquier collider que sea parte de la hitbox del arma
        if (collision.GetComponentInParent<MeleeHit>() != null) return;

        // Ignorar colliders hijos del propio Player
        if (collision.transform.IsChildOf(transform)) return;

        // Si la layer del objeto que colisiona es la layer del arma, ignorar
        if (weaponLayer != -1 && collision.gameObject.layer == weaponLayer) return;

        // Si la layer del objeto que colisiona es la layer de enemigos, daño
        if (enemyLayer != -1)
        {
            // permitir colisiones con hijos: subimos por la jerarquía buscando la layer
            Transform t = collision.transform;
            while (t != null)
            {
                if (t.gameObject.layer == enemyLayer)
                {
                    TakeDamage(1);
                    return;
                }
                t = t.parent;
            }
        }

        // Fallback por componente (si no hay layer configurada correctamente)
        if (collision.GetComponentInParent<FatherEnemy>() != null)
        {
            TakeDamage(1);
            return;
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
