using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(PatrollingAI))]
public class ProgrammerEnemy : RangedEnemy
{
    #region Attack Routine
    protected override System.Collections.IEnumerator AttackRoutine(NavMeshAgent agent, Player player, bool is360)
    {
        if (_attackObject == null || agent == null || player == null) yield break;

        _isAttacking = true;

        // Detener movimiento mientras ataca
        float previousAgentSpeed = agent.speed;
        agent.isStopped = true;

        yield return StartCoroutine(Delay());
        yield return StartCoroutine(Forward());
        yield return StartCoroutine(Return());

        if (WaitForAttackComplete && _attackObject != null)
        {
            yield return StartCoroutine(WaitForAttackObjectToReturn());
        }

        _attackObject.SetActive(false);
        _isAttacking = false;

        // Reanudar movimiento después del ataque
        agent.isStopped = false;
        agent.speed = previousAgentSpeed;
    }

    private System.Collections.IEnumerator Delay()
    {
        yield return new WaitForSeconds(_delay);
        _attackObject.SetActive(true);
    }
    #endregion

    #region Attack Movement
    private System.Collections.IEnumerator Forward()
    {
        Vector3 initialPosition = _attackObject.transform.position;
        Vector3 movementDirection = (Vector3)_currentDirection.normalized;
        Vector3 endPosition = initialPosition + movementDirection * _distance;
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
    #endregion

    #region Helpers
    private System.Collections.IEnumerator WaitForAttackObjectToReturn()
    {
        while (true)
        {
            Vector3 localAttackPosition = transform.InverseTransformPoint(_attackObject.transform.position);
            if (Mathf.Approximately(localAttackPosition.x, 0f) && Mathf.Approximately(localAttackPosition.y, 0f))
                break;
            yield return null;
        }
    }
    #endregion
}
