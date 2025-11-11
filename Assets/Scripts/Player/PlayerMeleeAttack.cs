using System.Collections;
using UnityEngine;

public class PlayerMeleeAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject meleeArea;     // El collider o hitbox del ataque
    [SerializeField] private Transform playerTransform; // Referencia al jugador para dirección

    [Header("Attack Settings")]
    [SerializeField] private float attackDuration = 0.3f;    // Tiempo que el ataque está activo
    [SerializeField] private float attackMoveDistance = 1f;  // Qué tan lejos se mueve el golpe
    [SerializeField] private float attackSpeed = 10f;        // Qué tan rápido se mueve
    [SerializeField] private KeyCode attackKey = KeyCode.J;  // Tecla de ataque (puede cambiarse)

    private bool isAttacking = false;
    private Vector3 originalPos;

    private void Start()
    {
        if (meleeArea == null)
        {
            Debug.LogError("No se asignó el área de ataque (meleeArea).");
            enabled = false;
            return;
        }

        originalPos = meleeArea.transform.localPosition;
        meleeArea.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(attackKey) && !isAttacking)
        {
            StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack()
    {
        isAttacking = true;
        meleeArea.SetActive(true);

        // Dirección del ataque según la rotación del jugador
        float direction = playerTransform.localRotation.y == 0 ? 1 : -1;
        Vector3 attackPos = originalPos + Vector3.right * direction * attackMoveDistance;

        // Movimiento hacia adelante
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * attackSpeed;
            meleeArea.transform.localPosition = Vector3.Lerp(originalPos, attackPos, t);
            yield return null;
        }

        yield return new WaitForSeconds(attackDuration);

        // Regresar a la posición original
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * attackSpeed;
            meleeArea.transform.localPosition = Vector3.Lerp(attackPos, originalPos, t);
            yield return null;
        }

        meleeArea.SetActive(false);
        isAttacking = false;
    }
}
