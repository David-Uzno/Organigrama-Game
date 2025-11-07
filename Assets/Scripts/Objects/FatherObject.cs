using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FatherObject : MonoBehaviour, ICollectible
{
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                Collect(player);
                Destroy(gameObject);
            }
        }
    }

    public abstract void Collect(Player player);
}