using System.Collections.Generic;
using UnityEngine;

public enum SpawnLocation
{
    High,
    Medium,
    Low
}

[CreateAssetMenu(fileName = "NewBossAction", menuName = "Boss/Boss Action")]
public class BossActionSO : ScriptableObject
{
    [Header("Action Basic Info")]
    public string actionName;

    [Header("Spawn Settings")]
    public SpawnLocation spawnLocation;
    public GameObject enemyPrefab;
    public int spawnCount = 1;
    public float delayBetweenSpawns = 0.5f;

    [Header("Timing Settings")]
    //public float actionDuration = 2f;
    public float cooldownAfterAction = 1f;
}