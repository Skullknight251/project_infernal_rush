using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class LowPlaceEnemySpawnList : ScriptableObject
{
    [SerializeField] public List<EnemySO> enemySOList;
}
