using UnityEngine;
using UnityEngine.AI;

public abstract class RangedAttack : FatherEnemy, ICanMove
{
    [Header("References")]
    [SerializeField] protected GameObject _attackObject;

    [Header("Attack Settings")]
    [SerializeField] protected float _distance = 5f;
    [SerializeField] protected float _duration = 1f;
    [SerializeField] protected float _delay = 0.3f;

    [Header("Attack Mode")]
    [Range(4, 100)]
    [SerializeField] protected int _cardinalDivisions = 4;
    [SerializeField] protected bool _canShoot360 = false;
    [SerializeField] protected float _postAttackMoveDelay = 0f;

    [Header("Attack Completion")]
    [SerializeField] protected bool _waitForAttackComplete = false;

    protected PatrollingAI _patrollingAI;
    protected bool _isAttacking;
    protected Vector2 _currentDirection = Vector2.up;

    public virtual int CardinalDivisions => Mathf.Max(_cardinalDivisions, 4);
    public virtual bool CanShoot360 => _canShoot360;
    public virtual float PostAttackMoveDelay => _postAttackMoveDelay;
    public virtual bool WaitForAttackComplete => _waitForAttackComplete;

    public virtual bool CanMove
    {
        get
        {
            if (!WaitForAttackComplete)
                return !_isAttacking;
            else
                return !_isAttacking && !HasPendingAttackInstances();
        }
    }

    protected virtual void Start()
    {
        _patrollingAI = GetComponent<PatrollingAI>();
        if (_patrollingAI != null)
            _patrollingAI.OnAttackRequested += HandleAttackRequested;

        if (_attackObject != null)
            _attackObject.SetActive(false);
    }

    protected virtual void OnDestroy()
    {
        if (_patrollingAI != null)
            _patrollingAI.OnAttackRequested -= HandleAttackRequested;
    }

    protected virtual void HandleAttackRequested(NavMeshAgent agent, Player player, Vector2 direction, bool is360)
    {
        if (_isAttacking) return;
        _currentDirection = direction;
        StartCoroutine(AttackRoutine(agent, player, is360));
    }

    protected abstract System.Collections.IEnumerator AttackRoutine(NavMeshAgent agent, Player player, bool is360);
    protected virtual bool HasPendingAttackInstances() => false;
}
