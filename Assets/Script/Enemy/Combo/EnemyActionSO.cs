using System.Collections;
using UnityEngine;

public abstract class EnemyActionSO : ScriptableObject
{
    [Header("Basic Action Info")]
    public string actionName;
    public abstract IEnumerator ExecuteAction(Enemy enemy);
}