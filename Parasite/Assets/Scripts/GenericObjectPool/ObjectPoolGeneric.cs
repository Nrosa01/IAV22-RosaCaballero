using System.Collections.Generic;
using UnityEngine;

public class MonoPoolGeneric<T> : ObjectPoolGeneric<T> where T : MonoBehaviour, IPoolable, new()
{
    public MonoPoolGeneric(T prefab) : base(prefab) { }

    public override T Spawn()
    {
        T pooolableObject;
        if (pool.Count > 0)
        {
            pooolableObject = pool.Dequeue();
            pooolableObject.transform.SetParent(null);
            pooolableObject.gameObject.SetActive(true);
        }
        else
        {
            pooolableObject = GameObject.Instantiate(prefab);
            GameObject.DontDestroyOnLoad(pooolableObject);
            pooolableObject.OnCreated();
        }
        pooolableObject.OnSpawn();

        return pooolableObject;
    }

    public override void Despawn(T instance)
    {
        if (!instance.isActiveAndEnabled)
            Debug.LogWarning("Estas intentando devolver un objeto dos veces a la pool");
        else
        {
            instance.gameObject.SetActive(false);
            pool.Enqueue(instance);
            instance.OnDespawn();
        }
    }
}

public class ObjectPoolGeneric<T> where T : IPoolable, new()
{
    protected Queue<T> pool;
    protected private T prefab;

    public ObjectPoolGeneric() { }
    public ObjectPoolGeneric(T prefab)
    {
        pool = new Queue<T>();
        this.prefab = prefab;
    }


    public virtual T Spawn()
    {
        T pooolableObject;
        if (pool.Count > 0)
        {
            pooolableObject = pool.Dequeue();
        }
        else
        {
            pooolableObject = new T();
        }
        return pooolableObject;
    }

    //TODO
    public virtual void Despawn(T instance)
    {
        pool.Enqueue(instance);
        instance.OnDespawn();
    }

}

public interface IPoolable
{
    void Despawn();
    void OnSpawn();
    void OnDespawn();
    void OnCreated();
}
