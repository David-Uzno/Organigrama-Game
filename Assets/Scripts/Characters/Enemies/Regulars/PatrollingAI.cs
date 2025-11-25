using System;
using NavMeshPlus.Extensions;
using UnityEngine;
using UnityEngine.AI;

public interface ICanMove
{
    bool CanMove { get; }
}

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
    private ICanMove _canMoveComponent;

    [Header("Attacks")]
    [SerializeField] protected float _attackCooldown = 1.5f;

    protected NavMeshAgent _agent;
    private float _reactionTimer = 0f;
    private bool _isChasingPlayer = false;
    protected float _attackTimer = 0f;

    // Evento que notifica cuando el agente debe ejecutar un ataque
    public event Action<NavMeshAgent, Player, Vector2, bool> OnAttackRequested;

    // Propiedades públicas mínimas para acceso externo si se necesita
    public NavMeshAgent Agent => _agent;
    public Player Player => _player;
    public float MinDistanceToPlayer => _minDistanceToPlayer;

    protected virtual void Start()
    {
        InitAgent();
        Invoke(nameof(FindPlayer), 0.1f);
        _canMoveComponent = GetComponent<ICanMove>();
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

        // Actualiza el temporizador de ataque
        if (_attackTimer > 0f)
            _attackTimer -= Time.deltaTime;
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
        // Si existe un componente que controla el movimiento y no puede moverse, no hacer nada
        if (_canMoveComponent != null && !_canMoveComponent.CanMove)
            return;

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
        // Si existe un componente que controla el movimiento y no puede moverse, no hacer nada
        if (_canMoveComponent != null && !_canMoveComponent.CanMove)
            return;

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

            if (_attackTimer <= 0f)
            {
                GetAttackConfig(out int divisions, out bool is360);
                Vector2 direction = CalculateCardinalDirection(_player.transform.position - transform.position, divisions, is360);
                OnAttackRequested?.Invoke(_agent, _player, direction, is360);
                _attackTimer = _attackCooldown;
            }
        }
    }

    private void GetAttackConfig(out int divisions, out bool is360)
    {
        is360 = false;
        divisions = 4;

        if (OnAttackRequested != null)
        {
            foreach (var onAttackDelegate in OnAttackRequested.GetInvocationList())
            {
                var target = onAttackDelegate.Target as MonoBehaviour;
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
    }

    private Vector2 CalculateCardinalDirection(Vector2 direction, int divisions, bool is360)
    {
        if (is360)
            return direction.normalized;

        float angle = Mathf.Atan2(direction.y, direction.x);
        if (angle < 0) angle += 2 * Mathf.PI;
        float sector = 2 * Mathf.PI / divisions;
        int sectorIndex = Mathf.FloorToInt((angle + sector / 2f) / sector) % divisions;
        float snappedAngle = sectorIndex * sector;
        return new Vector2(Mathf.Cos(snappedAngle), Mathf.Sin(snappedAngle)).normalized;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _visionRadius);
    }
}
