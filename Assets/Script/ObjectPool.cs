using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : MonoBehaviour
{
    private Queue<T> pool = new Queue<T>();
    private GameObject prefab;
    private int initialSize;

    public ObjectPool(GameObject prefab, int initialSize)
    {
        this.prefab = prefab;
        this.initialSize = initialSize;
        InitializePool();
    }

    void InitializePool()
    {
        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = GameObject.Instantiate(prefab);
            T component = obj.GetComponent<T>();
            obj.SetActive(false);
            pool.Enqueue(component);
        }
    }

    public T Get()
    {
        if (pool.Count > 0)
        {
            T obj = pool.Dequeue();
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            GameObject newObj = GameObject.Instantiate(prefab);
            return newObj.GetComponent<T>();
        }
    }

    public void Return(T obj)
    {
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
    }
}
