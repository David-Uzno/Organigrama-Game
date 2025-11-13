using System.Collections;
using UnityEngine;

public class PlayerMeleeAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject meleeArea;      // Collider o hitbox del ataque
    [SerializeField] private Transform playerTransform; // Referencia al jugador

    [Header("Attack Settings (Arco de Mouse)")]
    [SerializeField] private float attackDuration = 0.2f;   // Pausa al final
    [SerializeField] private float arcSweepAngle = 120f;    // Ángulo total del barrido
    [SerializeField] private float rotationSpeed = 6f;      // Velocidad del swing
    [SerializeField] private float attackRadius = 1f;       // Radio del ataque
    [SerializeField] private KeyCode attackKey = KeyCode.J;

    private bool isAttacking = false;
    private Camera mainCamera;
    private Transform followParent; // pivote temporal que sigue al jugador

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

        // --- 1. Calcular dirección del mouse ---
        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;

        Vector3 direction = (mouseWorld - playerTransform.position).normalized;
        float centerAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // --- 2. Crear pivote temporal (simula ser el padre) ---
        if (followParent == null)
        {
            GameObject pivot = new GameObject("MeleePivot");
            followParent = pivot.transform;
        }

        followParent.position = playerTransform.position;
        meleeArea.transform.SetParent(followParent); // ahora el arma “sigue” al pivote

        meleeArea.SetActive(true);

        float startAngle = centerAngle - arcSweepAngle / 2f;
        float endAngle = centerAngle + arcSweepAngle / 2f;
        float t = 0f;

        // --- 3. Movimiento del arco (rotación local respecto al pivote) ---
        while (t < 1f)
        {
            // Hacer que el pivote siga al jugador cada frame
            followParent.position = playerTransform.position;

            t += Time.deltaTime * rotationSpeed;
            float currentAngle = Mathf.Lerp(startAngle, endAngle, t);

            // Rotar el pivote, lo que mueve el arma
            followParent.rotation = Quaternion.Euler(0, 0, currentAngle);

            // La espada está a un radio fijo del centro
            meleeArea.transform.localPosition = Vector3.right * attackRadius;
            meleeArea.transform.localRotation = Quaternion.identity;

            yield return null;
        }

        // --- 4. Finalización ---
        yield return new WaitForSeconds(attackDuration);

        meleeArea.transform.SetParent(null); // quitar el “parent temporal”
        meleeArea.SetActive(false);

        isAttacking = false;
    }
}
