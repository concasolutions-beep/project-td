using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private GameObject prefab;
    private Queue<GameObject> pool = new Queue<GameObject>();
    private Transform parent;

    public ObjectPool(GameObject prefab, int initialSize, Transform parent = null)
    {
        this.prefab = prefab;
        this.parent = parent;

        for (int i = 0; i < initialSize; i++)
        {
            CreateObject();
        }
    }

    private GameObject CreateObject()
    {
        GameObject obj = GameObject.Instantiate(prefab, parent);
        obj.SetActive(false);
        pool.Enqueue(obj);
        return obj;
    }

    public GameObject Get()
    {
        if (pool.Count == 0)
        {
            CreateObject();
        }

        GameObject obj = pool.Dequeue();
        obj.SetActive(true);
        return obj;
    }

    public void Release(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}