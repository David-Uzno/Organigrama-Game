using UnityEngine;

public class PlayerAttack : MeleeAttack
{
    private Transform _playerTransform;
    private Camera _mainCamera;

    protected override void Start()
    {
        base.Start();
        _playerTransform = transform;
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !_isAttacking)
        {
            StartCoroutine(Attack());
        }
    }

    protected override float GetAttackCenterAngle()
    {
        Vector3 mouseWorld = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;
        Vector3 direction = (mouseWorld - _playerTransform.position).normalized;
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }
}
