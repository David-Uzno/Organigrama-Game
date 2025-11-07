using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Status Lifes
public interface IHealable
{
    void RecoverLife(int amount);
}

public interface IDamageable
{
    void TakeDamage(float damage);
}
#endregion 

#region Combat
public interface IShootable
{
    void FireWeapon();
    bool CanShoot();
}
#endregion

#region Objects
public interface ICollectible
{
    void Collect(Player player);
}
#endregion