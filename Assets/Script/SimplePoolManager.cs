using System.Collections.Generic;
using UnityEngine;

public class SimplePoolManager : MonoBehaviour
{
    public static SimplePoolManager Instance { get; private set; }
    private Dictionary<GameObject, Queue<GameObject>> poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { 
            Destroy(gameObject); 
            return; 
        }
        Instance = this;
    }

    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null) return null;

        if (!poolDictionary.ContainsKey(prefab))
        {
            poolDictionary.Add(prefab, new Queue<GameObject>());
        }

        GameObject obj;

        if (poolDictionary[prefab].Count > 0)
        {
            obj = poolDictionary[prefab].Dequeue();
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true); 
        }
        else
        {
            obj = Instantiate(prefab, position, rotation);

            PoolMember member = obj.GetComponent<PoolMember>();
            if (member == null) member = obj.AddComponent<PoolMember>();
            member.myPrefab = prefab;
        }

        return obj;
    }

    public void Despawn(GameObject obj)
    {
        PoolMember member = obj.GetComponent<PoolMember>();
        if (member == null)
        {
            Destroy(obj);
            return;
        }
        obj.SetActive(false);
        if (poolDictionary.ContainsKey(member.myPrefab))
        {
            poolDictionary[member.myPrefab].Enqueue(obj);
        }
        else
        {
            Destroy(obj); 
        }
    }
}
public class PoolMember : MonoBehaviour
{
    public GameObject myPrefab;
}
