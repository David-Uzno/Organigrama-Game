using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(PatrollingAI))]
public class SoundEnginnerEnemy : MeleeAttack, IDamageable
{
    private PatrollingAI _patrollingAI;
    [Header("Enemy Stats")]
    [SerializeField] private int _maxHealth = 3;
    private int _currentHealth;

    // Dirección usada por GetAttackCenterAngle()
    private Vector2 _currentDirection = Vector2.right;

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    protected override void Start()
    {
        base.Start();

        _patrollingAI = GetComponent<PatrollingAI>();
        if (_patrollingAI != null)
        {
            // Asegurate que la firma del evento coincide con esta
            _patrollingAI.OnAttackRequested += OnAttackRequested;
            Debug.Log($"{name}: Suscripto a OnAttackRequested");
        }
        else
        {
            Debug.LogWarning($"{name}: No encontré PatrollingAI en el mismo GameObject.");
        }
    }

    private void OnDestroy()
    {
        if (_patrollingAI != null)
            _patrollingAI.OnAttackRequested -= OnAttackRequested;
    }

    // Firma esperada: (NavMeshAgent agent, Player player, Vector2 direction, bool is360)
    private void OnAttackRequested(NavMeshAgent agent, Player player, Vector2 direction, bool is360)
    {
        Debug.Log($"{name}: OnAttackRequested recibida. isAttacking={_isAttacking}");
        if (_isAttacking) return;

        // Preferir la dirección que envía la IA; si es zero, calcularla desde player
        if (direction != Vector2.zero)
            _currentDirection = direction.normalized;
        else if (player != null)
            _currentDirection = ((Vector2)player.transform.position - (Vector2)transform.position).normalized;
        else
            _currentDirection = transform.right; // fallback

        // Iniciamos la corrutina de ataque que usa _currentDirection
        StartCoroutine(AttackRoutine(agent));
    }

    private IEnumerator AttackRoutine(NavMeshAgent agent)
    {
        if (agent == null)
        {
            Debug.LogWarning($"{name}: AttackRoutine abortada porque agent es null.");
            yield break;
        }

        _isAttacking = true;
        float prevSpeed = agent.speed;
        bool prevStopped = agent.isStopped;
        agent.isStopped = true;

        // (Opcional) pequeña espera si querés sincronizar animación
        // yield return new WaitForSeconds(0.05f);

        // Llamamos al Attack() de la base. Antes de llamarlo, GetAttackCenterAngle() leerá _currentDirection.
        yield return StartCoroutine(Attack());

        // Esperar un frame extra para asegurar limpieza de hitbox/anim
        yield return null;

        agent.isStopped = prevStopped;
        agent.speed = prevSpeed;
        _isAttacking = false;

        Debug.Log($"{name}: AttackRoutine finalizada.");
    }

    // IMPORTANTE: MeleeAttack utiliza GetAttackCenterAngle() para rotar el pivot/hitbox.
    // Aquí devolvemos el ángulo en grados correspondiente a _currentDirection.
    protected override float GetAttackCenterAngle()
    {
        // 0 degrees points to Vector.right in MeleeAttack (porque la hitbox se coloca en Vector3.right)
        if (_currentDirection == Vector2.zero) return 0f;
        float ang = Mathf.Atan2(_currentDirection.y, _currentDirection.x) * Mathf.Rad2Deg;
        return ang;
    }

    // Implementación de la interfaz de daño (usa float como dijiste)
    public void TakeDamage(float amount)
    {
        int damage = Mathf.Max(0, Mathf.RoundToInt(amount));
        _currentHealth -= damage;
        Debug.Log($"{name}: TakeDamage({amount}) => health {_currentHealth}/{_maxHealth}");

        // Feedback visual o anim puede ir aquí (parpadeo, knockback...)
        // StartCoroutine(FlashHit());

        if (_currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        Debug.Log($"{name}: Die()");
        // Reemplaza por anim + delay si querés
        Destroy(gameObject);
    }

    // (Opcional) si querés ver la dirección en el editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 pos = transform.position;
        Vector3 dir3 = new Vector3(_currentDirection.x, _currentDirection.y, 0f);
        Gizmos.DrawLine(pos, pos + dir3.normalized * 1.5f);
    }
}
