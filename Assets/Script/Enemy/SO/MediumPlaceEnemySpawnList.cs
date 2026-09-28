using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class MediumPlaceEnemySpawnList : ScriptableObject
{
    [SerializeField] public List<EnemySO> enemySOList;
}
