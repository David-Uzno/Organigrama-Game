using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class EyeEnemy : FatherEnemy
{
    #region Variables
    [Header("Shooting")]
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private float _bulletSpeed = 5f;
    [SerializeField] private float _timeBetweenShots = 2f;
    [SerializeField] private float _fireDelay = 0.25f;
    private bool _canShoot = true;
    private float _lastShotTime = 0f;

    [Header("Detection")]
    [SerializeField] private bool _playerDetectedEnabled;
    [SerializeField] private Vector2 _firePosition = Vector2.right;
    [SerializeField] private float _detectionDistance = 5f;
    #endregion

    #region Unity Methods
    private void Update()
    {
        if (_playerDetectedEnabled)
            ShootWhenPlayerDetected();
        else
            ShootRegularly();
    }

    private void OnDrawGizmos()
    {
        DrawDetectionGizmo();
    }
    #endregion

    #region Shooting Functions
    private void ShootWhenPlayerDetected()
    {
        RaycastHit2D playerDetected = Physics2D.Raycast(transform.position, _firePosition, _detectionDistance);

        if (playerDetected.collider != null && playerDetected.collider.CompareTag("Player"))
            Shoot();
    }

    private void ShootRegularly()
    {
        if (_canShoot && Time.time > _lastShotTime + _timeBetweenShots)
        {
            _lastShotTime = Time.time;
            StartCoroutine(ShootWithDelay(_fireDelay));
        }
    }

    private void Shoot()
    {
        GameObject bullet = Instantiate(_bulletPrefab, transform.position, Quaternion.identity);
        Rigidbody2D bulletRB = bullet.GetComponent<Rigidbody2D>();

        if (bulletRB != null)
            bulletRB.linearVelocity = _firePosition * _bulletSpeed;

        _canShoot = false;
        StartCoroutine(ResetShoot());
    }

    private IEnumerator ShootWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Shoot();
    }

    private IEnumerator ResetShoot()
    {
        yield return new WaitForSeconds(_timeBetweenShots);
        _canShoot = true;
    }
    #endregion

    #region Gizmo
    private void DrawDetectionGizmo()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + _firePosition * _detectionDistance);
    }
    #endregion
}