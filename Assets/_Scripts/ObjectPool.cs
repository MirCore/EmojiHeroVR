using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;
    [SerializeField] private List<GameObject> PooledObjects;
    [SerializeField] private GameObject ObjectToPool;
    [SerializeField] private int AmountToPool;

    private void Awake()
    {
        // Create Singleton of this Class
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }
    
    /// <summary>
    /// Instantiate Object Pool
    /// </summary>
    private void Start()
    {
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
        for(int i = 0; i < AmountToPool; i++)
        {
            if(!PooledObjects[i].activeInHierarchy)
            {
                return PooledObjects[i];
            }
        }
        throw new NullReferenceException("Pooled Object returned null");
    }
}
