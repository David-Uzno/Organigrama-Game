using System.Collections;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected GameObject _meleeHitbox;

    [Header("Attack Settings")]
    [SerializeField] protected float _attackActiveTime = 0.2f;
    [SerializeField] protected float _attackArcAngle = 120f;
    [SerializeField] protected float _attackArcSpeed = 11.5f;
    [SerializeField] protected float _attackDistance = 1f;

    [Header("Layer & Collision")]
    [SerializeField] protected string _weaponLayer = "PlayerWeapon";
    [SerializeField] protected bool _ignorePlayerCollision = true;

    protected Transform _hitboxOriginalParent;
    protected int _weaponLayerIndex = -1;
    protected bool _isAttacking;

    protected virtual void Start()
    {
        _hitboxOriginalParent = _meleeHitbox.transform.parent;

        if (!ValidateReferences()) return;
        InitializeMeleeHitbox();
        AssignWeaponLayer();
    }

    protected IEnumerator Attack()
    {
        _isAttacking = true;

        float centerAngle = GetAttackCenterAngle();
        GameObject pivot = CreatePivot(centerAngle);

        PrepareHitboxForAttack(pivot);

        float startAngle = centerAngle - _attackArcAngle / 2f;
        float endAngle = centerAngle + _attackArcAngle / 2f;

        yield return RotateHitboxArc(pivot, startAngle, endAngle);

        yield return new WaitForSeconds(_attackActiveTime);

        ResetHitbox();
        Destroy(pivot);
        _isAttacking = false;
    }

    protected virtual float GetAttackCenterAngle()
    {
        return 0f;
    }

    protected void PrepareHitboxForAttack(GameObject pivot)
    {
        _meleeHitbox.transform.SetParent(pivot.transform, false);
        _meleeHitbox.SetActive(true);
    }

    protected IEnumerator RotateHitboxArc(GameObject pivot, float startAngle, float endAngle)
    {
        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * _attackArcSpeed;
            float currentAngle = Mathf.Lerp(startAngle, endAngle, elapsedTime);
            pivot.transform.rotation = Quaternion.Euler(0f, 0f, currentAngle);
            _meleeHitbox.transform.SetLocalPositionAndRotation(Vector3.right * _attackDistance, Quaternion.identity);
            yield return null;
        }
    }

    protected void ResetHitbox()
    {
        _meleeHitbox.transform.SetParent(_hitboxOriginalParent, true);
        _meleeHitbox.SetActive(false);
    }

    protected bool ValidateReferences()
    {
        if (_meleeHitbox == null)
        {
            Debug.LogError("MeleeAttack: meleeHitbox no asignado.");
            enabled = false;
            return false;
        }
        return true;
    }

    protected void InitializeMeleeHitbox()
    {
        _meleeHitbox.SetActive(false);
        if (_meleeHitbox.CompareTag("Enemy"))
        {
            _meleeHitbox.tag = "Untagged";
            Debug.LogWarning("MeleeAttack: meleeHitbox tenía tag 'Enemy' — lo he puesto 'Untagged'. Usa una tag/layer específica para armas.");
        }
        if (_meleeHitbox.TryGetComponent<Collider2D>(out var col))
            col.isTrigger = true;
    }

    protected void AssignWeaponLayer()
    {
        if (string.IsNullOrEmpty(_weaponLayer)) return;
        _weaponLayerIndex = LayerMask.NameToLayer(_weaponLayer);
        if (_weaponLayerIndex == -1)
        {
            Debug.LogWarning($"MeleeAttack: la layer '{_weaponLayer}' no existe. Crea la layer en __Project Settings > Tags and Layers__.");
            return;
        }
        
        SetLayerRecursively(_meleeHitbox.transform, _weaponLayerIndex);

        if (_ignorePlayerCollision)
        {
            int playerLayer = gameObject.layer;
            if (playerLayer != _weaponLayerIndex)
                Physics2D.IgnoreLayerCollision(_weaponLayerIndex, playerLayer, true);
        }
    }

    protected GameObject CreatePivot(float centerAngle)
    {
        GameObject pivot = new("MeleePivot");
        pivot.transform.SetParent(transform, true);
        pivot.transform.position = transform.position;
        if (_weaponLayerIndex != -1) pivot.layer = _weaponLayerIndex;
        return pivot;
    }

    protected void SetLayerRecursively(Transform root, int layer)
    {
        root.gameObject.layer = layer;
        foreach (Transform child in root)
            SetLayerRecursively(child, layer);
    }
}
