using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sistema de pooling propio para reusar gameobjects
/// </summary>
/// <typeparam name="T"></typeparam>
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

/// <summary>
/// Sistema de pooling para reusar cualquier tipo de objeto en general, "new()" es para asegurar que el objeto
/// que se guarda en la pool esta en el heap en vez de en el stack y que por tanto no se destruya mientras
/// este en la pool de forma aleatoria
/// </summary>
/// <typeparam name="T"></typeparam>
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
