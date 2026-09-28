using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class HighPlaceEnemySpawnList : ScriptableObject
{
    [SerializeField] public List<EnemySO> enemySOList; 
   
}
