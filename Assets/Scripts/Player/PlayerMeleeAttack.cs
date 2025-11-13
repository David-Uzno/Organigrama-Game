using System.Collections;
using UnityEngine;

public class PlayerMeleeAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject meleeArea;     // El collider o hitbox del ataque
    [SerializeField] private Transform playerTransform; // Referencia al jugador

    [Header("Attack Settings (Arco de Mouse)")]
    [SerializeField] private float attackDuration = 0.2f;    // Pausa al final
    [SerializeField] private float arcSweepAngle = 120f;     // Ángulo total del barrido
    [SerializeField] private float rotationSpeed = 6f;       // Velocidad del swing
    [SerializeField] private float attackRadius = 1f;        // Radio del arco (distancia al jugador)
    [SerializeField] private KeyCode attackKey = KeyCode.J;

    private bool isAttacking = false;
    private Camera mainCamera;

    private void Start()
    {
        if (meleeArea == null)
        {
            Debug.LogError("No se asignó el área de ataque (meleeArea).");
            enabled = false;
            return;
        }

        mainCamera = Camera.main;
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

        // --- 1. Obtener dirección del mouse ---
        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;
        Vector3 direction = (mouseWorld - playerTransform.position).normalized;

        // Ángulo hacia el mouse
        float centerAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // --- 2. Definir ángulo inicial y final del arco ---
        float startAngle = centerAngle - arcSweepAngle / 2f;
        float endAngle = centerAngle + arcSweepAngle / 2f;

        float t = 0f;
        meleeArea.SetActive(true);

        // --- 3. Movimiento del arco (rotación alrededor del jugador) ---
        while (t < 1f)
        {
            t += Time.deltaTime * rotationSpeed;
            float currentAngle = Mathf.Lerp(startAngle, endAngle, t);

            // Calcular la posición en arco (basada en el radio)
            Vector3 offset = new Vector3(
                Mathf.Cos(currentAngle * Mathf.Deg2Rad),
                Mathf.Sin(currentAngle * Mathf.Deg2Rad),
                0
            ) * attackRadius;

            meleeArea.transform.position = playerTransform.position + offset;
            meleeArea.transform.rotation = Quaternion.Euler(0, 0, currentAngle);

            yield return null;
        }

        // --- 4. Finalización ---
        yield return new WaitForSeconds(attackDuration);
        meleeArea.SetActive(false);
        isAttacking = false;
    }
}
