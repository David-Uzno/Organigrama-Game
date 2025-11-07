using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : FatherObject
{
    [SerializeField] private int _recoveryAmount = 1;

    public delegate void HeartCollected();
    public static event HeartCollected OnHeartCollected;

    public override void Collect(Player player)
    {
        player.RecoverLife(_recoveryAmount);
        NotifyHeartCollection();
    }

    private void NotifyHeartCollection()
    {
        if (OnHeartCollected != null)
        {
            OnHeartCollected.Invoke();
        }
    }
}