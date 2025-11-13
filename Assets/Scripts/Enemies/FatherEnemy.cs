using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FatherEnemy : MonoBehaviour, IDamageable
{
    #region Variables
    [SerializeField] private float _life = 1f;
    [SerializeField] private EnemyDropConfig _dropConfig;
    #endregion

    #region Unity Methods
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Usar otherCollider para inspeccionar el objeto que chocó con el enemigo
        Collider2D otherCol = collision.otherCollider;
        if (otherCol == null) return;

        // Si la colisión viene del hitbox del arma, NO contamos eso como "Player hit"
        if (otherCol.GetComponentInParent<MeleeHit>() != null)
        {
            Debug.Log($"FatherEnemy: impacto de arma {otherCol.gameObject.name} ignorado.");
            return;
        }

        // Si el otro pertenece al Player (directamente o en sus padres), perder vida
        if (otherCol.GetComponentInParent<Player>() != null)
        {
            GameManager.Instance.LoseLife();
            return;
        }

        Debug.Log($"FatherEnemy colisionó con: {otherCol.gameObject.name} tag:{otherCol.gameObject.tag}");
    }
    #endregion

    #region Damage
    public void TakeDamage(float damage)
    {
        _life -= damage;
        if (_life <= 0)
        {
            DropItem();
           Destroy(gameObject);
        }
    }
    #endregion

    #region Drop
    private void DropItem()
    {
        if (_dropConfig != null && _dropConfig.DropItems.Count > 0)
        {
            foreach (var dropItem in _dropConfig.DropItems)
            {
                float chance = Random.value * 100;
                if (chance < dropItem.Chance)
                {
                    Instantiate(dropItem.Item, transform.position, Quaternion.identity);
                    break;
                }
            }
        }
    }
    #endregion
}