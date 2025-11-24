using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(PatrollingAI))]
public class ArtistEnemy : RangedEnemy
{
    [Header("Artist")]
    [SerializeField] private float _destroyDelay = 1.5f;

    protected override System.Collections.IEnumerator AttackRoutine(NavMeshAgent agent, Player player, bool is360)
    {
        if (_attackObject == null || agent == null || player == null) yield break;

        _isAttacking = true;

        // Detener movimiento mientras ataca
        float prevSpeed = agent.speed;
        agent.isStopped = true;

        yield return new WaitForSeconds(_delay);

        // Instanciar y lanzar el objeto de ataque
        GameObject attackInstance = Instantiate(_attackObject, transform.position, Quaternion.identity);
        attackInstance.SetActive(true);

        Vector3 startPosition = transform.position;
        Vector3 movementDirection = (Vector3)_currentDirection.normalized;
        Vector3 endPosition = startPosition + movementDirection * _distance;

        float timeElapsed = 0f;
        while (timeElapsed < 1f)
        {
            if (attackInstance == null) yield break;
            attackInstance.transform.position = Vector3.Lerp(startPosition, endPosition, timeElapsed);
            timeElapsed += Time.deltaTime / _duration;
            yield return null;
        }
        if (attackInstance != null)
            attackInstance.transform.position = endPosition;

        // Destruir el objeto después de un tiempo
        if (attackInstance != null)
            Destroy(attackInstance, _destroyDelay);

        _isAttacking = false;

        // Reanudar movimiento después del ataque
        agent.isStopped = false;
        agent.speed = prevSpeed;
    }
}
