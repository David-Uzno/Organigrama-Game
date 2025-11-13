using UnityEngine;
using System.Collections.Generic;

public class MeleeHit : MonoBehaviour
{
    [SerializeField] private float damage = 1f;
    [SerializeField] private bool debugHit = false;

    private HashSet<Collider2D> damagedEnemies = new HashSet<Collider2D>();

    private void OnEnable()
    {
        // Al iniciar el ataque, limpiamos la lista para permitir nuevo daño
        damagedEnemies.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Solo enemigos
        if (!other.CompareTag("Enemy")) return;

        // Evita dañar varias veces al mismo enemigo en un mismo ataque
        if (damagedEnemies.Contains(other)) return;

        IDamageable enemy = other.GetComponent<IDamageable>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            damagedEnemies.Add(other);

            if (debugHit)
                Debug.Log($"Golpeaste a {other.name} con {damage} de daño");
        }
    }
}
