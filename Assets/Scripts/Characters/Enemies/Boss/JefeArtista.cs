using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JefeArtista : MonoBehaviour, IDamageable
{
    [Header("Vida")]
    [SerializeField] private float _life = 25f;

    [Header("Daño Visual")]
    [SerializeField] private SpriteRenderer _blinkRenderer;
    [SerializeField] private float _blinkDuration = 0.05f;
    [SerializeField] private int _blinkCount = 3;

    [Header("Movimiento / Salto")]
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private float _jumpHeight = 8f;
    [SerializeField] private float _jumpDuration = 0.4f;

    [Header("Timers")]
    [SerializeField] private float _minJumpDelay = 1.2f;
    [SerializeField] private float _maxJumpDelay = 2.8f;
    [SerializeField] private float _delayBeforeDescend = 0.2f;

    [Header("Daño del Salto")]
    [SerializeField] private float _jumpUpDamage = 0f;
    [SerializeField] private float _jumpDownDamage = 1f;
    [SerializeField] private float _damageRadius = 1f;
    [SerializeField] private LayerMask _playerMask;

    [Header("Animación y Sonidos")]
    [SerializeField] private Animator _animator;
    [SerializeField] private AudioSource _audioJump;

    private bool _canJump = true;

    [Header("Carga de Ataque (Color)")]
    [SerializeField] private float _chargeTime = 2f;
    [SerializeField] private SpriteRenderer _bodyRenderer;

    [SerializeField] private Color _redColor = Color.red;
    [SerializeField] private Color _blueColor = Color.cyan;
    [SerializeField] private Color _greenColor = Color.green;

    private enum AttackType { Red, Blue, Green }
    private AttackType _nextAttack;

    [Header("Mancha de Veneno")]
    [SerializeField] private GameObject _poisonPrefab;
    [SerializeField] private float _poisonOffsetY = -1f;

    [SerializeField] public float _delayBeforeLoad = 2f;
    private void Start()
    {
        if (_playerTransform == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) _playerTransform = p.transform;
        }

        StartCoroutine(AttackCycle());
        ScheduleNextJump();
    }


    public void TakeDamage(float damage)
    {
        _life -= damage;
        StartCoroutine(BlinkDamage());

        if (_life <= 0)
            Die();
    }

    private IEnumerator BlinkDamage()
    {
        if (_blinkRenderer == null) yield break;

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
        GameManager.Instance.SetBossState("Nivel2", true);

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
    // ==============================
    //        CICLO DE ATAQUE
    // ==============================
    private IEnumerator AttackCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2f, 4f));
            SelectRandomAttack();
            yield return ChargeAttack();
        }
    }

    private void SelectRandomAttack()
    {
        _nextAttack = (AttackType)Random.Range(0, 3);
    }

    private IEnumerator ChargeAttack()
    {
        switch (_nextAttack)
        {
            case AttackType.Red: _bodyRenderer.color = _redColor; break;
            case AttackType.Blue: _bodyRenderer.color = _blueColor; break;
            case AttackType.Green: _bodyRenderer.color = _greenColor; break;
        }

        yield return new WaitForSeconds(_chargeTime);
    }

    // ==============================
    //             SALTOS
    // ==============================

    private void ScheduleNextJump()
    {
        float delay = Random.Range(_minJumpDelay, _maxJumpDelay);
        Invoke(nameof(StartJump), delay);
    }

    private void StartJump()
    {
        if (!_canJump) return;
        StartCoroutine(JumpRoutine());
    }

    private IEnumerator JumpRoutine()
    {
        _canJump = false;

        Vector3 start = transform.position;
        Vector3 peak = start + Vector3.up * _jumpHeight;

        _audioJump?.Play();
        _animator?.SetBool("Jump", true);

        // SUBE
        yield return MoveSmooth(start, peak, _jumpDuration);
        DamagePlayerArea(_jumpUpDamage);

        yield return new WaitForSeconds(_delayBeforeDescend);

        // BAJA
        Vector3 fallPos = new Vector3(_playerTransform.position.x, _playerTransform.position.y, 0);

        yield return MoveSmooth(peak, fallPos, _jumpDuration);

        _animator?.SetBool("Jump", false);

        DamagePlayerArea(_jumpDownDamage);
        SpawnPoison();

        _canJump = true;
        ScheduleNextJump();
    }

    private IEnumerator MoveSmooth(Vector3 from, Vector3 to, float t)
    {
        float el = 0f;

        while (el < t)
        {
            el += Time.deltaTime;
            transform.position = Vector3.Lerp(from, to, el / t);
            yield return null;
        }

        transform.position = to;
    }

    // ==============================
    //        DAÑO EN ÁREA
    // ==============================

    private void DamagePlayerArea(float dmg)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _damageRadius, _playerMask);

        foreach (var h in hits)
        {
            if (h.CompareTag("Player"))
            {
                GameManager.Instance.LoseLife();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, _damageRadius);
    }

    // ==============================
    //            VENENO
    // ==============================

    private void SpawnPoison()
    {
        if (_poisonPrefab == null) return;

        Vector3 pos = transform.position + new Vector3(0, _poisonOffsetY, 0);
        GameObject p = Instantiate(_poisonPrefab, pos, Quaternion.identity);

        if (p.TryGetComponent<SpriteRenderer>(out var r))
            r.color = _bodyRenderer.color;
    }
}
