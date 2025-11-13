using NavMeshPlus.Extensions;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PatrollingAI : MonoBehaviour
{
    private Player _player;
    [SerializeField] private float _minDistanceToPlayer = 2f;
    [SerializeField] private float _reactionTime = 0.5f;
    [SerializeField] private float _visionRadius = 7.5f;

    private NavMeshAgent _agent;
    private float _reactionTimer = 0f;

    private void Start()
    {
        InitAgent();
        FindPlayer();
    }

    private void InitAgent()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.updatePosition = false;
        _agent.updateUpAxis = false;
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
        var nextPos = _agent.nextPosition;
        nextPos.z = 0f;
        transform.position = nextPos;

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
            if (distanceToPlayer > _minDistanceToPlayer && _reactionTimer >= _reactionTime)
            {
                _agent.SetDestination(_player.transform.position);
                _reactionTimer = 0f;
            }
        }
        else
        {
            // Si el Player está fuera del campo de visión, se detiene
            _agent.ResetPath();
        }
    }

    private void HandleAgentStop()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);
        // Si está dentro de la distancia mínima, se detiene
        if (distanceToPlayer <= _minDistanceToPlayer)
        {
            _agent.ResetPath();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _visionRadius);
    }
}
