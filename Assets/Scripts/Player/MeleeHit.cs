using UnityEngine;

public class MeleeHit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<IDamageable>()?.TakeDamage(1);
        }
    }
}
