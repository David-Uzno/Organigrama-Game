using NavMeshPlus.Extensions;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PatrollingAI : MonoBehaviour
{
    [SerializeField] private Transform[] _patrolPoints;
    private int _currentPointIndex = 0;
    private NavMeshAgent _agent;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.updatePosition = false;
        _agent.updateUpAxis = false;

        _agent.SetDestination(_patrolPoints[_currentPointIndex].position);
    }

    private void Update()
    {
        // Actualiza la posición del GameObject manualmente y fuerza Z a 0
        var nextPos = _agent.nextPosition;
        nextPos.z = 0f;
        transform.position = nextPos;

        // Evita la rotación en el eje Z
        var rotationEulerAngles = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(rotationEulerAngles.x, 0f, rotationEulerAngles.z);

        if (!_agent.pathPending && _agent.remainingDistance < 0.1f)
        {
            _currentPointIndex = (_currentPointIndex + 1) % _patrolPoints.Length;
            _agent.SetDestination(_patrolPoints[_currentPointIndex].position);
        }
    }
}
