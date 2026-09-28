using UnityEngine;


[CreateAssetMenu()]
public class EnemySO : ScriptableObject
{
    [SerializeField] public GameObject enemyPrefab;
    [SerializeField] public string name;
}
