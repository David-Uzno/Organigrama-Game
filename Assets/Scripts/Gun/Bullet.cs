using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] float _speed = 7.5f;
    [SerializeField] float _timeLife = 0.75f;

    private void Update()
    {
        // Movimiento de Bala
        transform.Translate(Vector2.up * _speed * Time.deltaTime);

        //Tiempo de Vida de Bala
        StartCoroutine(DestroyBullet());
    }

    IEnumerator DestroyBullet()
    {
        // Destrucción de Bala
        yield return new WaitForSeconds(_timeLife);
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(1);
            }
        }
    }
}