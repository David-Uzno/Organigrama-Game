using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(PatrollingAI))]
public class ProgrammerEnemy : FatherEnemy
{
    [Header("Programmer")]
    private PatrollingAI _patrollingAI;
    private bool _isAttacking = false;

    [Header("Attack References")]
    [SerializeField] private GameObject _attackObject;

    [Header("Attack Mode")]
    [Range(4, 100)]
    [SerializeField] private int _cardinalDivisions = 4;
    public int CardinalDivisions => Mathf.Max(_cardinalDivisions, 4);
    [SerializeField] private bool _canShoot360 = false;
    public bool CanShoot360 => _canShoot360;

    [Header("Attack Settings")]
    [SerializeField] private float _distance = 5f;
    [SerializeField] private float _duration = 1f;
    [SerializeField] private float _delay = 0.3f;

    private Vector2 _currentDirection = Vector2.up;

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

    private void HandleAttackRequested(NavMeshAgent agent, Player player, Vector2 direction, bool is360)
    {
        if (_isAttacking) return;
        _currentDirection = direction;
        StartCoroutine(AttackUpwards(agent, player));
    }

    private System.Collections.IEnumerator AttackUpwards(NavMeshAgent agent, Player player)
    {
        if (_attackObject == null || agent == null || player == null) yield break;

        _isAttacking = true;

        // Detener movimiento mientras ataca
        float prevSpeed = agent.speed;
        agent.isStopped = true;

        yield return StartCoroutine(Delay());
        yield return StartCoroutine(Forward());
        yield return StartCoroutine(Return());

        _attackObject.SetActive(false);
        _isAttacking = false;

        // Reanudar movimiento después del ataque
        agent.isStopped = false;
        agent.speed = prevSpeed;
    }

    private System.Collections.IEnumerator Delay()
    {
        yield return new WaitForSeconds(_delay);
        _attackObject.SetActive(true);
    }

    private System.Collections.IEnumerator Forward()
    {
        Vector3 initialPosition = _attackObject.transform.position;
        Vector3 dir = (Vector3)_currentDirection.normalized;
        Vector3 endPosition = initialPosition + dir * _distance;
        yield return StartCoroutine(MoveObject(initialPosition, endPosition));
    }

    private System.Collections.IEnumerator Return()
    {
        Vector3 endPosition = _attackObject.transform.position;
        yield return StartCoroutine(MoveObjectToLocalZero(endPosition));
    }

    private System.Collections.IEnumerator MoveObject(Vector3 startPosition, Vector3 targetPosition)
    {
        float timeElapsed = 0f;
        while (timeElapsed < 1f)
        {
            _attackObject.transform.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed);
            timeElapsed += Time.deltaTime / _duration;
            yield return null;
        }
        _attackObject.transform.position = targetPosition;
    }

    private System.Collections.IEnumerator MoveObjectToLocalZero(Vector3 startPosition)
    {
        float timeElapsed = 0f;
        while (timeElapsed < 1f)
        {
            Vector3 targetPosition = transform.position;
            _attackObject.transform.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed);
            timeElapsed += Time.deltaTime / _duration;
            yield return null;
        }
        _attackObject.transform.position = transform.position;
    }
}
