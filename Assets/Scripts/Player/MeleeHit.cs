using UnityEngine;
using System.Collections.Generic;

public class MeleeHit : MonoBehaviour
{
    [SerializeField] private float damage = 1f;
    private HashSet<Collider2D> damaged = new HashSet<Collider2D>();

    private void OnEnable()
    {
        damaged.Clear(); // Reset al iniciar cada ataque
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        if (damaged.Contains(other)) return;
        damaged.Add(other);

        if (other.TryGetComponent<IDamageable>(out var dmg))
        {
            dmg.TakeDamage(damage);
            Debug.Log($"Golpeaste a {other.name}");
        }
    }
}
