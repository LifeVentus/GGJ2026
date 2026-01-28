using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;

public interface IObjectPool
{
    // 活跃在场景中的数量
    int ActiveCount{get;}
    // 池内总数量(active + idle)
    int TotalCount{get;}
    // 闲置在池内数量
    int IdleCount{get;}
}
public class ObjectPool<T> : IObjectPool where T : MonoBehaviour
{   
    private Queue<T> pool = new Queue<T>();
    private T prefab;
    private Transform parent;
    private int activeCount = 0;
    public int ActiveCount => activeCount;
    public int TotalCount => pool.Count + activeCount;
    public int IdleCount => pool.Count;
    public ObjectPool(T prefab, int initialSize, Transform parent = null)
    {
        this.prefab = prefab;
        this.parent = parent;
        for(int i = 0; i < initialSize; i++)
        {
            T obj = GameObject.Instantiate(prefab, parent);
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
        }
    }
    public T Get()
    {
        T obj = pool.Count > 0 ? pool.Dequeue() : GameObject.Instantiate(prefab, parent);
        obj.gameObject.SetActive(true);
        activeCount++;
        return obj;
    }

    public void Return(T obj)
    {
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
        activeCount--;
    }

}