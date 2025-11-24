using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyDropConfig", menuName = "ScriptablesObjects/Enemies/DropConfig")]
public class EnemyDropConfig : ScriptableObject
{
    [System.Serializable] public class DropItem
    {
        public GameObject Item;
        [Range(0, 100)] public float Chance;
    }
    public List<DropItem> DropItems;
}