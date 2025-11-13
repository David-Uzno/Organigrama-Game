using UnityEngine;
using System.Collections.Generic;

public class MeleeHit : MonoBehaviour
{
    [SerializeField] private float damage = 0f;
    [SerializeField] private bool debugHit = false;

    // Ahora trackeamos GameObjects (no colliders) para evitar múltiples hits por enemigo
    private HashSet<GameObject> damagedTargets = new HashSet<GameObject>();

    private void OnEnable()
    {
        damagedTargets.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;

        // Ignorar cualquier collider que pertenezca al Player (o a sus padres)
        if (other.GetComponentInParent<Player>() != null)
        {
            if (debugHit) Debug.Log($"MeleeHit: Ignorado collider del Player: {other.name}");
            return;
        }

        // Buscar el padre más cercano que implemente IDamageable
        Transform t = other.transform;
        IDamageable targetDamageable = null;
        Transform targetTransform = null;
        while (t != null)
        {
            targetDamageable = t.GetComponent<IDamageable>();
            if (targetDamageable != null)
            {
                targetTransform = t;
                break;
            }
            t = t.parent;
        }

        if (targetDamageable == null || targetTransform == null) return;

        // Asegurarnos de no dañar al Player aunque implemente IDamageable
        if (targetTransform.GetComponent<Player>() != null) return;

        GameObject targetGO = targetTransform.gameObject;
        if (damagedTargets.Contains(targetGO)) return; // ya fue dañado en este ataque

        // Aplicar daño y registrar
        targetDamageable.TakeDamage(damage);
        damagedTargets.Add(targetGO);

        if (debugHit)
            Debug.Log($"MeleeHit: Golpeaste a {targetGO.name} con {damage} de daño");
    }

    // útil si quieres reiniciar manualmente sin desactivar el objeto
    public void ResetDamagedTargets() => damagedTargets.Clear();
}
