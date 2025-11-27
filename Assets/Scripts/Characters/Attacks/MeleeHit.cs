using UnityEngine;
using System.Collections.Generic;

public class MeleeHit : MonoBehaviour
{
    [SerializeField] private float damage = 1f;

    private HashSet<Collider2D> damaged = new HashSet<Collider2D>();
    private GameObject owner; // ← agregamos quién es el dueño

    public void SetOwner(GameObject ownerObject)
    {
        owner = ownerObject;
    }

    private void OnEnable()
    {
        damaged.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Evitar dañarse a sí mismo
        if (owner != null && other.gameObject == owner)
            return;

        // Dañar solo a lo que implemente IDamageable
        if (other.TryGetComponent<IDamageable>(out var dmg))
        {
            // Si ya lo golpeamos durante este ataque → no repetir
            if (damaged.Contains(other)) return;
            damaged.Add(other);

            dmg.TakeDamage(damage);
            Debug.Log($"{name} hizo daño a {other.name} por {damage}");
        }
    }
}