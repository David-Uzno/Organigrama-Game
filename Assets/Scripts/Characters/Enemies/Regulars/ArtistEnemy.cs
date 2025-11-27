using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(PatrollingAI))]
public class ArtistEnemy : RangedAttack
{
    #region Variables
    [Header("Artist")]
    [SerializeField] private int _shotsPerAttack = 1;
    [SerializeField] private float _delayBetweenShots = 0.1f;
    [SerializeField] private float _destroyDelay = 0;

    private GameObject[] _activeAttackInstances;
    #endregion

    #region Attack Routine
    protected override System.Collections.IEnumerator AttackRoutine(NavMeshAgent agent, Player player, bool is360)
    {
        if (_attackObject == null || agent == null || player == null) yield break;

        _isAttacking = true;
        float previousAgentSpeed = agent.speed;
        agent.isStopped = true;

        yield return new WaitForSeconds(_delay);

        _activeAttackInstances = new GameObject[_shotsPerAttack];

        yield return ExecuteAttackShots(_activeAttackInstances);

        if (WaitForAttackComplete)
            yield return StartCoroutine(WaitForAllAttackInstancesDestroyed(_activeAttackInstances));

        _activeAttackInstances = null;
        _isAttacking = false;
        agent.isStopped = false;
        agent.speed = previousAgentSpeed;
    }
    #endregion

    #region Attack Logic
    private System.Collections.IEnumerator ExecuteAttackShots(GameObject[] attackInstances)
    {
        for (int i = 0; i < _shotsPerAttack; i++)
        {
            attackInstances[i] = CreateAttackInstance();
            if (attackInstances[i] != null)
                yield return MoveAndFinalizeAttackInstance(attackInstances[i]);

            if (i < _shotsPerAttack - 1)
                yield return new WaitForSeconds(_delayBetweenShots);
        }
    }
    #endregion

    #region Attack Instance Management
    private GameObject CreateAttackInstance()
    {
        var instance = Instantiate(_attackObject, transform.position, Quaternion.identity);

        // Dirección hacia donde va a disparar
        float angle = Mathf.Atan2(_currentDirection.y, _currentDirection.x) * Mathf.Rad2Deg;

        // Como el sprite apunta ARRIBA, sumamos 90 grados
        instance.transform.rotation = Quaternion.Euler(0, 0, angle - 90f + 180f);

        instance.SetActive(true);
        return instance;
    }

    private System.Collections.IEnumerator MoveAndFinalizeAttackInstance(GameObject attackInstance)
    {
        Vector3 startPosition = transform.position;
        Vector3 movementDirection = _currentDirection.normalized;
        Vector3 endPosition = startPosition + movementDirection * _distance;

        yield return MoveAttackInstance(attackInstance, startPosition, endPosition);
        FinalizeAttackInstance(attackInstance);
    }

    private System.Collections.IEnumerator MoveAttackInstance(GameObject attackInstance, Vector3 start, Vector3 end)
    {
        float timeElapsed = 0f;
        while (timeElapsed < 1f)
        {
            if (attackInstance == null) yield break;
            attackInstance.transform.position = Vector3.Lerp(start, end, timeElapsed);
            timeElapsed += Time.deltaTime / _duration;
            yield return null;
        }
        if (attackInstance != null)
            attackInstance.transform.position = end;
    }

    private void FinalizeAttackInstance(GameObject attackInstance)
    {
        if (attackInstance != null)
            Destroy(attackInstance, _destroyDelay);
    }
    #endregion

    #region Helpers
    protected override bool HasPendingAttackInstances()
    {
        return WaitForAttackComplete && _activeAttackInstances != null;
    }

    private System.Collections.IEnumerator WaitForAllAttackInstancesDestroyed(GameObject[] attackInstances)
    {
        while (true)
        {
            bool anyAlive = false;
            foreach (var inst in attackInstances)
            {
                if (inst != null)
                {
                    anyAlive = true;
                    break;
                }
            }
            if (!anyAlive) break;
            yield return null;
        }
    }
    #endregion
}
