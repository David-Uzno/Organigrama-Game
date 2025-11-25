using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(PatrollingAI))]
public class SoundEnginnerEnemy : MeleeAttack
{
    private PatrollingAI _patrollingAI;

    protected override void Start()
    {
        base.Start();
        _patrollingAI = GetComponent<PatrollingAI>();
        if (_patrollingAI != null)
            _patrollingAI.OnAttackRequested += OnAttackRequested;
    }

    private void OnDestroy()
    {
        if (_patrollingAI != null)
            _patrollingAI.OnAttackRequested -= OnAttackRequested;
    }

    private void OnAttackRequested(NavMeshAgent agent, Player player, Vector2 direction, bool is360)
    {
        if (!_isAttacking)
            StartCoroutine(AttackRoutine(agent, player));
    }

    private IEnumerator AttackRoutine(NavMeshAgent agent, Player player)
    {
        float previousSpeed = agent.speed;
        agent.isStopped = true;

        yield return StartCoroutine(Attack());

        agent.isStopped = false;
        agent.speed = previousSpeed;
    }
}
