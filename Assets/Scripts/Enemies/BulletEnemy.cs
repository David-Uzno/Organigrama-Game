using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BulletEnemy : MonoBehaviour
{
    [SerializeField] float _timeLife = 5f;

    private void Update()
    {
        Destroy(gameObject, _timeLife);
    }
}