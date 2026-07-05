using UnityEngine;
using System.Collections.Generic;

public class PoolManager : MonoBehaviour
{

    public static PoolManager Instance;

    private Dictionary<GameObject, ObjectPool> pools = new Dictionary<GameObject, ObjectPool>();

    void Awake()
    {
        Instance = this;
    }

    public ObjectPool GetPool(GameObject prefab, int size = 10)
    {
        if (!pools.ContainsKey(prefab))
        {
            ObjectPool newPool = new ObjectPool(prefab, size, transform);
            pools.Add(prefab, newPool);
        }

        return pools[prefab];
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
