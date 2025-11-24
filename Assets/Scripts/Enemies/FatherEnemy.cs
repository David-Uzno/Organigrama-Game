using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class FatherEnemy : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    [SerializeField] private float _life = 1f;
    [SerializeField] private EnemyDropConfig _dropConfig;

    [Header("Layers")]
    [SerializeField] private string _playerLayerName = "Player";
    [SerializeField] private string _weaponLayerName = "PlayerWeapon";
    private int _playerLayer = -1;
    private int _weaponLayer = -1;

    private void Awake()
    {
        _playerLayer = LayerMask.NameToLayer(_playerLayerName);
        _weaponLayer = LayerMask.NameToLayer(_weaponLayerName);

        if (_playerLayer == -1)
            Debug.LogWarning($"FatherEnemy: Layer '{_playerLayerName}' no existe.");
        if (_weaponLayer == -1)
            Debug.LogWarning($"FatherEnemy: Layer '{_weaponLayerName}' no existe.");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var otherCollision = collision.otherCollider;
        if (otherCollision == null) return;

        if (IsWeaponCollision(otherCollision)) return;
        if (IsPlayerCollision(otherCollision)) return;
    }

    private bool IsWeaponCollision(Collider2D otherCollision)
    {
        if ((_weaponLayer != -1 && otherCollision.gameObject.layer == _weaponLayer) ||
            otherCollision.GetComponentInParent<MeleeHit>() != null)
        {
            return true;
        }
        return false;
    }

    private bool IsPlayerCollision(Collider2D otherCollision)
    {
        if ((_playerLayer != -1 && otherCollision.gameObject.layer == _playerLayer) ||
            otherCollision.GetComponentInParent<Player>() != null)
        {
            GameManager.Instance.LoseLife();
            return true;
        }
        return false;
    }

    public void TakeDamage(float damage)
    {
        _life -= damage;
        if (_life <= 0)
        {
            DropItem();
            Destroy(gameObject);
        }
    }

    private void DropItem()
    {
        if (_dropConfig == null || _dropConfig.DropItems.Count == 0) return;

        foreach (var dropItem in _dropConfig.DropItems)
        {
            if (Random.value * 100 < dropItem.Chance)
            {
                Instantiate(dropItem.Item, transform.position, Quaternion.identity);
                break;
            }
        }
    }
}
