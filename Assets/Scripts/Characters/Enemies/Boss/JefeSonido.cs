using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JefeSonido : MonoBehaviour, IDamageable
{
    [Header("Vida")]
    [SerializeField] private float _life = 25f;
    [SerializeField] private float _delayBeforeLoad = 2f;


    [Header("Daño Visual")]
    [SerializeField] private SpriteRenderer _blinkRenderer;
    [SerializeField] private float _blinkDuration = 0.05f;
    [SerializeField] private int _blinkCount = 3;

    [Header("Referencias")]
    [SerializeField] private SpriteRenderer _bodyRenderer;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Animator _animator;
    [SerializeField] private AudioSource _audioJump;

    // ==========================================================
    //                 ⚠️ NUEVOS PARÁMETROS DE DAÑO
    // ==========================================================

    [Header("Daño por Ataque")]
    [SerializeField] private float _dashDamage = 1f;
    [SerializeField] private float _jumpUpDamage = 0f;
    [SerializeField] private float _jumpDownDamage = 1f;
    [SerializeField] private float _projectileDamage = 1f;

    [Header("Daño Área del Salto")]
    [SerializeField] private float _damageRadius = 1f;
    [SerializeField] private LayerMask _playerMask;

    // ==========================================================
    //                     DASH Settings
    // ==========================================================
    [Header("DASH Settings")]
    [SerializeField] private float _dashSpeed = 12f;
    [SerializeField] private float _dashDuration = 0.4f;
    [SerializeField] private float _dashLoadTime = 2f;

    // ==========================================================
    //                     JUMP Settings
    // ==========================================================
    [Header("SALTOS Settings")]
    [SerializeField] private float _jumpHeight = 8f;
    [SerializeField] private float _jumpDuration = 0.4f;
    [SerializeField] private float _minJumpDelay = 1.2f;
    [SerializeField] private float _maxJumpDelay = 2.8f;
    [SerializeField] private float _delayBeforeDescend = 0.2f;

    // ==========================================================
    //                   Proyectiles
    // ==========================================================

    [Header("Proyectiles Caída")]
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private float _projectileSpeed = 6f;

    [Header("Colores de Carga")]
    [SerializeField] private Color _chargeStartColor = Color.white;
    [SerializeField] private Color _chargeRedColor = Color.red;

    private bool _canAct = true;

    private enum BossAttack
    {
        Dash,
        Jump
    }

    private void Start()
    {
        if (_playerTransform == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) _playerTransform = p.transform;
        }

        StartCoroutine(AttackLoop());
    }

    // ==========================================================
    //                  CICLO PRINCIPAL DE ATAQUE
    // ==========================================================
    private IEnumerator AttackLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(1.5f, 2.4f));

            if (!_canAct) continue;

            BossAttack next = (BossAttack)Random.Range(0, 2);

            if (next == BossAttack.Dash)
                yield return StartCoroutine(DashRoutine());
            else
                yield return StartCoroutine(JumpRoutine());
        }
    }

    // ==========================================================
    //                           DASH
    // ==========================================================
    private IEnumerator DashRoutine()
    {
        _canAct = false;

        // CARGA
        float t = 0;
        while (t < _dashLoadTime)
        {
            t += Time.deltaTime;
            _bodyRenderer.color = Color.Lerp(_chargeStartColor, _chargeRedColor, t / _dashLoadTime);
            yield return null;
        }

        // Direccion hacia el jugador
        Vector3 dir = (_playerTransform.position - transform.position).normalized;

        float time = 0;
        while (time < _dashDuration)
        {
            transform.position += dir * _dashSpeed * Time.deltaTime;

            // ⚠️ Daño durante el dash
            DamagePlayerArea(_dashDamage);

            time += Time.deltaTime;
            yield return null;
        }

        _bodyRenderer.color = _chargeStartColor;
        _canAct = true;
    }

    // ==========================================================
    //                          SALTO
    // ==========================================================
    private IEnumerator JumpRoutine()
    {
        _canAct = false;

        Vector3 start = transform.position;
        Vector3 peak = start + Vector3.up * _jumpHeight;

        _animator?.SetBool("Jump", true);
        _audioJump?.Play();

        // SUBE
        yield return MoveSmooth(start, peak, _jumpDuration);
        DamagePlayerArea(_jumpUpDamage);

        yield return new WaitForSeconds(_delayBeforeDescend);

        // BAJA hacia el jugador
        Vector3 target = _playerTransform.position;
        target.z = 0;

        yield return MoveSmooth(peak, target, _jumpDuration);

        _animator?.SetBool("Jump", false);

        // ⚠️ Daño fuerte al caer
        DamagePlayerArea(_jumpDownDamage);

        // Proyectiles
        ShootCrossProjectiles();

        _canAct = true;
    }

    // ==========================================================
    //                     PROYECTILES EN CRUZ
    // ==========================================================
    private void ShootCrossProjectiles()
    {
        if (_projectilePrefab == null) return;

        SpawnProjectile(Vector2.right);
        SpawnProjectile(Vector2.left);
        SpawnProjectile(Vector2.up);
        SpawnProjectile(Vector2.down);
    }

    private void SpawnProjectile(Vector2 dir)
    {
        GameObject p = Instantiate(_projectilePrefab, transform.position, Quaternion.identity);

        // Movimiento
        Rigidbody2D rb = p.GetComponent<Rigidbody2D>();
        rb.linearVelocity = dir * _projectileSpeed;

        // Destruir después de un tiempo
        Destroy(p, 2.5f);
    }


    // ==========================================================
    //                        DAÑO EN ÁREA
    // ==========================================================
    private void DamagePlayerArea(float dmg)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _damageRadius, _playerMask);

        foreach (var h in hits)
        {
            if (h.CompareTag("Player"))
                GameManager.Instance.LoseLife();
        }
    }

    private IEnumerator MoveSmooth(Vector3 from, Vector3 to, float time)
    {
        float t = 0;

        while (t < time)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(from, to, t / time);
            yield return null;
        }

        transform.position = to;
    }

    public void TakeDamage(float dmg)
    {
        _life -= dmg;
        StartCoroutine(BlinkDamage());

        if (_life <= 0)
            Die();
    }

    private IEnumerator BlinkDamage()
    {
        for (int i = 0; i < _blinkCount; i++)
        {
            _blinkRenderer.enabled = false;
            yield return new WaitForSeconds(_blinkDuration);
            _blinkRenderer.enabled = true;
            yield return new WaitForSeconds(_blinkDuration);
        }
    }

    private void Die()
    {
        StartCoroutine(HandleDeath());
    }
    private IEnumerator HandleDeath()
    {
        // GameObject Temporal
        GameObject tempObject = new GameObject("TempObjectForSceneLoad");

        // Importante: el tempScript DEBE SER este mismo tipo de boss
        JefeArtista tempScript = tempObject.AddComponent<JefeArtista>();

        // Pasamos el delay (si querés que sea igual al original)
        tempScript._delayBeforeLoad = _delayBeforeLoad;

        // Corrutina de la Instancia Temporal
        tempScript.StartCoroutine(tempScript.LoadSceneAfterDelay());

        // Marca que el boss fue derrotado
        GameManager.Instance.SetBossState("Nivel3", true);

        // Destruye el GameObject Original
        Destroy(gameObject);

        yield return null;
    }
    public IEnumerator LoadSceneAfterDelay()
    {
        yield return new WaitForSeconds(_delayBeforeLoad);

        SceneManager.LoadScene("LevelSelector");

        // Destruye el GameObject Temporal
        Destroy(gameObject);
    }
}
