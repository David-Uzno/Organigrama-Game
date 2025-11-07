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
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.LoseLife();
        }
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