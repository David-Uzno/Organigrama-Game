using System;
using NavMeshPlus.Extensions;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PatrollingAI : MonoBehaviour
{
    protected Player _player;

    [Header("Player Detection")]
    [SerializeField] protected float _minDistanceToPlayer = 2f;
    [SerializeField] private float _reactionTime = 0.5f;
    [SerializeField] private float _visionRadius = 7.5f;

    [Header("Patrol")]
    [SerializeField] private float _randomMoveRadius = 10f;
    [SerializeField] private float _patrolSpeed = 3f;

    [Header("Persecution")]
    [SerializeField] private float _chaseSpeed = 4f;

    [Header("Runtime State")]
    protected NavMeshAgent _agent;
    private float _reactionTimer = 0f;
    private bool _isChasingPlayer = false;

    public event Action<NavMeshAgent, Player, Vector2, bool> OnAttackRequested;

    // Propiedades públicas mínimas para acceso externo si se necesita
    public NavMeshAgent Agent => _agent;
    public Player Player => _player;
    public float MinDistanceToPlayer => _minDistanceToPlayer;

    protected virtual void Start()
    {
        InitAgent();
        Invoke(nameof(FindPlayer), 0.1f);
    }

    private void InitAgent()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.updatePosition = false;
        _agent.updateUpAxis = false;
        _agent.speed = _patrolSpeed;
    }

    private void FindPlayer()
    {
        _player = FindFirstObjectByType<Player>();
        if (_player == null)
        {
            Debug.LogWarning("No se encontró ningún objeto Player en la escena.");
        }
    }

    private void Update()
    {
        UpdatePositionAndRotation();

        if (_player != null)
        {
            HandlePlayerTracking();
            HandleAgentStop();
        }
    }

    private void UpdatePositionAndRotation()
    {
        // Actualiza la posición del GameObject manualmente y fuerza Z a 0
        var nextPosition = _agent.nextPosition;
        nextPosition.z = 0f;
        transform.position = nextPosition;

        // Evita la rotación en el eje Z
        var rotationEulerAngles = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(rotationEulerAngles.x, 0f, rotationEulerAngles.z);
    }

    private void HandlePlayerTracking()
    {
        _reactionTimer += Time.deltaTime;
        float distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);

        // Solo se acerca si el Player está dentro del campo de visión
        if (distanceToPlayer <= _visionRadius)
        {
            _isChasingPlayer = true;
            _agent.speed = _chaseSpeed;
            if (distanceToPlayer > _minDistanceToPlayer && _reactionTimer >= _reactionTime)
            {
                _agent.SetDestination(_player.transform.position);
                _reactionTimer = 0f;
            }
        }
        else
        {
            // Si el Player está fuera del campo de visión, patrulla de forma aleatoria
            if (_isChasingPlayer || !_agent.hasPath || _agent.remainingDistance < 0.5f)
            {
                SetRandomDestination();
                _isChasingPlayer = false;
                _agent.speed = _patrolSpeed;
            }
        }
    }

    private void SetRandomDestination()
    {
        Vector2 randomDirection = UnityEngine.Random.insideUnitCircle * _randomMoveRadius;
        Vector3 randomPosition = new(transform.position.x + randomDirection.x, transform.position.y + randomDirection.y, 0f);

        if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, _randomMoveRadius, NavMesh.AllAreas))
        {
            _agent.SetDestination(hit.position);
        }
    }

    protected virtual void HandleAgentStop()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);
        if (distanceToPlayer <= _minDistanceToPlayer)
        {
            _agent.ResetPath();
            CalculateAttackDirection(out Vector2 direction, out bool is360);
            if (OnAttackRequested != null)
            {
                OnAttackRequested(_agent, _player, direction, is360);
            }
        }
    }

    private void CalculateAttackDirection(out Vector2 direction, out bool is360)
    {
        is360 = false;
        int divisions = 4;
        direction = (_player.transform.position - transform.position);

        // Consultar divisiones y 360°
        if (OnAttackRequested != null)
        {
            foreach (var attackTargetDelegate in OnAttackRequested.GetInvocationList())
            {
                var target = attackTargetDelegate.Target as MonoBehaviour;
                if (target != null)
                {
                    var programmer = target as ProgrammerEnemy;
                    if (programmer != null)
                    {
                        if (programmer.CanShoot360)
                        {
                            is360 = true;
                            break;
                        }
                        divisions = programmer.CardinalDivisions;
                    }
                }
            }
        }

        if (!is360)
        {
            // Dirección cardinal configurable
            float angle = Mathf.Atan2(direction.y, direction.x);
            if (angle < 0) angle += 2 * Mathf.PI; // Asegura ángulo positivo
            float sector = 2 * Mathf.PI / divisions;
            int sectorIndex = Mathf.FloorToInt((angle + sector / 2f) / sector) % divisions;
            float snappedAngle = sectorIndex * sector;
            direction = new Vector2(Mathf.Cos(snappedAngle), Mathf.Sin(snappedAngle)).normalized;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _visionRadius);
    }
}
