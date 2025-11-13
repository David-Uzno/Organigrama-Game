using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FatherEnemy : MonoBehaviour, IDamageable
{
    #region Variables
    [SerializeField] private float _life = 1f;
    [SerializeField] private EnemyDropConfig _dropConfig;

    // Layers
    [SerializeField] private string playerLayerName = "Player";
    [SerializeField] private string weaponLayerName = "PlayerWeapon";
    private int playerLayer = -1;
    private int weaponLayer = -1;
        #endregion

    private void Awake()
    {
        playerLayer = LayerMask.NameToLayer(playerLayerName);
        weaponLayer = LayerMask.NameToLayer(weaponLayerName);

        if (playerLayer == -1)
            Debug.LogWarning($"FatherEnemy: la layer '{playerLayerName}' no existe.");
        if (weaponLayer == -1)
            Debug.LogWarning($"FatherEnemy: la layer '{weaponLayerName}' no existe.");
    }

    #region Unity Methods
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D otherCol = collision.otherCollider;
        if (otherCol == null) return;

        // Si la colisión viene del hitbox del arma (por layer o componente), ignorar
        if (weaponLayer != -1 && otherCol.gameObject.layer == weaponLayer)
        {
            Debug.Log($"FatherEnemy: impacto de arma (layer) {otherCol.gameObject.name} ignorado.");
            return;
        }
        if (otherCol.GetComponentInParent<MeleeHit>() != null)
        {
            Debug.Log($"FatherEnemy: impacto de arma (component) {otherCol.gameObject.name} ignorado.");
            return;
        }

        // Si el otro collider pertenece a la layer del Player, contar daño al jugador
        if (playerLayer != -1 && otherCol.gameObject.layer == playerLayer)
        {
            GameManager.Instance.LoseLife();
            return;
        }

        // Fallback: si el collider pertenece al Player por componente
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