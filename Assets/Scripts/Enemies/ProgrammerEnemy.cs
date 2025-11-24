using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(PatrollingAI))]
public class ProgrammerEnemy : FatherEnemy
{
    [Header("Programmer")]
    [SerializeField] private GameObject _attackObject;
    [SerializeField] private float _attackDistance = 5f;
    [SerializeField] private float _attackDuration = 1f;
    [SerializeField] private Vector2 _attackDirection = Vector2.up;
    [SerializeField] private float _attackDelay = 0.3f;

    private bool _isAttacking = false;
    private PatrollingAI _patrollingAI;

    private void Start()
    {
        _patrollingAI = GetComponent<PatrollingAI>();
        if (_patrollingAI != null)
        {
            _patrollingAI.OnAttackRequested += HandleAttackRequested;
        }

        if (_attackObject != null)
        {
            _attackObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (_patrollingAI != null)
        {
            _patrollingAI.OnAttackRequested -= HandleAttackRequested;
        }
    }

    private void HandleAttackRequested(NavMeshAgent agent, Player player)
    {
        if (_isAttacking) return;
        // Iniciar coroutine de ataque con las referencias actuales
        StartCoroutine(AttackUpwards(agent, player));
    }

    private System.Collections.IEnumerator AttackUpwards(NavMeshAgent agent, Player player)
    {
        if (_attackObject == null || agent == null || player == null) yield break;

        _isAttacking = true;

        // Detener movimiento mientras ataca
        float prevSpeed = agent.speed;
        agent.isStopped = true;

        yield return StartCoroutine(AttackDelay());
        yield return StartCoroutine(AttackForward());
        yield return StartCoroutine(AttackReturn());

        _attackObject.SetActive(false);
        _isAttacking = false;

        // Reanudar movimiento después del ataque
        agent.isStopped = false;
        agent.speed = prevSpeed;
    }

    private System.Collections.IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(_attackDelay);
        _attackObject.SetActive(true);
    }

    private System.Collections.IEnumerator AttackForward()
    {
        Vector3 initialAttackPosition = _attackObject.transform.position;
        Vector3 direction = (Vector3)_attackDirection.normalized;
        Vector3 endAttackPosition = initialAttackPosition + direction * _attackDistance;
        yield return StartCoroutine(MoveAttackObject(initialAttackPosition, endAttackPosition));
    }

    private System.Collections.IEnumerator AttackReturn()
    {
        Vector3 endAttackPosition = _attackObject.transform.position;
        yield return StartCoroutine(MoveAttackObjectToLocalZero(endAttackPosition));
    }

    private System.Collections.IEnumerator MoveAttackObject(Vector3 startPosition, Vector3 targetPosition)
    {
        float timeElapsed = 0f;
        while (timeElapsed < 1f)
        {
            _attackObject.transform.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed);
            timeElapsed += Time.deltaTime / _attackDuration;
            yield return null;
        }
        _attackObject.transform.position = targetPosition;
    }

    private System.Collections.IEnumerator MoveAttackObjectToLocalZero(Vector3 startPosition)
    {
        float timeElapsed = 0f;
        while (timeElapsed < 1f)
        {
            Vector3 targetPosition = transform.position;
            _attackObject.transform.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed);
            timeElapsed += Time.deltaTime / _attackDuration;
            yield return null;
        }
        _attackObject.transform.position = transform.position;
    }
}
