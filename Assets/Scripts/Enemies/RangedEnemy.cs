using UnityEngine;
using UnityEngine.AI;

public abstract class RangedEnemy : FatherEnemy
{
    [Header("Ranged Attack References")]
    [SerializeField] protected GameObject _attackObject;

    [Header("Ranged Attack Settings")]
    [SerializeField] protected float _distance = 5f;
    [SerializeField] protected float _duration = 1f;
    [SerializeField] protected float _delay = 0.3f;

    protected PatrollingAI _patrollingAI;
    protected bool _isAttacking = false;
    protected Vector2 _currentDirection = Vector2.up;

    protected virtual void Start()
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

    protected virtual void OnDestroy()
    {
        if (_patrollingAI != null)
        {
            _patrollingAI.OnAttackRequested -= HandleAttackRequested;
        }
    }

    protected virtual void HandleAttackRequested(NavMeshAgent agent, Player player, Vector2 direction, bool is360)
    {
        if (_isAttacking) return;
        _currentDirection = direction;
        StartCoroutine(AttackRoutine(agent, player, is360));
    }

    protected abstract System.Collections.IEnumerator AttackRoutine(NavMeshAgent agent, Player player, bool is360);
}
