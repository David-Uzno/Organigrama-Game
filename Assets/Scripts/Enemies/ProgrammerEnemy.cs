using UnityEngine;

public class Programmer : MonoBehaviour
{
    [SerializeField] private GameObject _attackObject;
    [SerializeField] private float _attackDistance = 5f;
    [SerializeField] private float _attackDuration = 1f;

    private bool _isAttacking = false;

    private void Start()
    {
        if (_attackObject != null)
        {
            _attackObject.SetActive(false);
        }   
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !_isAttacking)
        {
            StartCoroutine(AttackUpwards());
        }
    }

    private System.Collections.IEnumerator AttackUpwards()
    {
        if (_attackObject == null) yield break;

        _isAttacking = true;
        _attackObject.SetActive(true);

        Vector3 initialAttackPosition = _attackObject.transform.position;
        Vector3 endAttackPosition = initialAttackPosition + Vector3.up * _attackDistance;

        // Movimiento ida
        yield return StartCoroutine(MoveAttackObject(initialAttackPosition, endAttackPosition));
        // Movimiento vuelta
        yield return StartCoroutine(MoveAttackObject(endAttackPosition, initialAttackPosition));

        _attackObject.SetActive(false);
        _isAttacking = false;
    }

    private System.Collections.IEnumerator MoveAttackObject(Vector3 from, Vector3 to)
    {
        float timeElapsed = 0f;
        while (timeElapsed < 1f)
        {
            _attackObject.transform.position = Vector3.Lerp(from, to, timeElapsed);
            timeElapsed += Time.deltaTime / _attackDuration;
            yield return null;
        }
        _attackObject.transform.position = to;
    }
}
