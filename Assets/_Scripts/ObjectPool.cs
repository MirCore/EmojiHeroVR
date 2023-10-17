using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities;

public class ObjectPool : Singleton<ObjectPool>
{
    [SerializeField] private List<GameObject> PooledObjects;
    [SerializeField] private GameObject ObjectToPool;
    [SerializeField] private int AmountToPool;
    
    /// <summary>
    /// Instantiate Object Pool
    /// </summary>
    private void Start()
    {
        ObjectToPool.SetActive(false);
        PooledObjects = new List<GameObject>();
        for (int i = 0; i < AmountToPool; i++)
        {
            GameObject tmp = Instantiate(ObjectToPool);
            tmp.SetActive(false);
            PooledObjects.Add(tmp);
        }
    }

    public GameObject GetPooledObject()
    {
        foreach (GameObject obj in PooledObjects.Where(obj => !obj.activeInHierarchy))
        {
            return obj;
        }

        throw new NullReferenceException("Pooled Object returned null");
    }
}
