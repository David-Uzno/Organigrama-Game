using System.Collections;
using UnityEngine;

public class PlayerMeleeAttack : MonoBehaviour
{
    [SerializeField] private GameObject meleeArea; // hitbox (con Collider2D)
    [SerializeField] private float attackDuration = 0.2f;
    [SerializeField] private float arcSweepAngle = 120f;
    [SerializeField] private float rotationSpeed = 6f;
    [SerializeField] private float attackRadius = 1f;
    private KeyCode attackKey = KeyCode.Mouse0;
    [SerializeField] private string weaponLayerName = "PlayerWeapon";
    [SerializeField] private bool autoIgnorePlayerCollision = true;

    private Transform playerTransform;
    private Camera mainCamera;
    private Transform originalParent;
    private int weaponLayer = -1;
    private bool isAttacking;

    private void Start()
    {
        if (meleeArea == null)
        {
            Debug.LogError("PlayerMeleeAttack: meleeArea no asignado.");
            enabled = false;
            return;
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
        {
            Debug.LogError("PlayerMeleeAttack: no se encontró GameObject con tag 'Player'.");
            enabled = false;
            return;
        }

        playerTransform = playerObj.transform;
        mainCamera = Camera.main;
        originalParent = meleeArea.transform.parent;
        meleeArea.SetActive(false);

        // Evitar que la hitbox esté taggeada como Enemy por error
        if (meleeArea.CompareTag("Enemy"))
        {
            meleeArea.tag = "Untagged";
            Debug.LogWarning("PlayerMeleeAttack: meleeArea tenía tag 'Enemy' — lo he puesto 'Untagged'. Usa una tag/layer específica para armas.");
        }

        // Forzar que el collider del arma sea Trigger (recomendado para hitboxes)
        var col = meleeArea.GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;

        // Asignar layer si existe
        if (!string.IsNullOrEmpty(weaponLayerName))
        {
            weaponLayer = LayerMask.NameToLayer(weaponLayerName);
            if (weaponLayer == -1)
            {
                Debug.LogWarning($"PlayerMeleeAttack: la layer '{weaponLayerName}' no existe. Crea la layer en __Project Settings > Tags and Layers__.");
            }
            else
            {
                SetLayerRecursively(meleeArea.transform, weaponLayer);
                if (autoIgnorePlayerCollision && playerObj != null)
                {
                    int playerLayer = playerObj.layer;
                    if (playerLayer != weaponLayer)
                        Physics2D.IgnoreLayerCollision(weaponLayer, playerLayer, true);
                }
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(attackKey) && !isAttacking)
            StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        isAttacking = true;

        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;
        Vector3 direction = (mouseWorld - playerTransform.position).normalized;
        float centerAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        GameObject pivot = new GameObject("MeleePivot");
        pivot.transform.SetParent(playerTransform, worldPositionStays: true);
        pivot.transform.position = playerTransform.position;
        if (weaponLayer != -1) pivot.layer = weaponLayer;

        meleeArea.transform.SetParent(pivot.transform, worldPositionStays: false);
        meleeArea.SetActive(true);

        float startAngle = centerAngle - arcSweepAngle / 2f;
        float endAngle = centerAngle + arcSweepAngle / 2f;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * rotationSpeed;
            float currentAngle = Mathf.Lerp(startAngle, endAngle, t);
            pivot.transform.rotation = Quaternion.Euler(0f, 0f, currentAngle);
            meleeArea.transform.localPosition = Vector3.right * attackRadius;
            meleeArea.transform.localRotation = Quaternion.identity;
            yield return null;
        }

        yield return new WaitForSeconds(attackDuration);

        meleeArea.transform.SetParent(originalParent, worldPositionStays: true);
        meleeArea.SetActive(false);

        Destroy(pivot);
        isAttacking = false;
    }

    private void SetLayerRecursively(Transform root, int layer)
    {
        root.gameObject.layer = layer;
        foreach (Transform child in root)
            SetLayerRecursively(child, layer);
    }
}
        