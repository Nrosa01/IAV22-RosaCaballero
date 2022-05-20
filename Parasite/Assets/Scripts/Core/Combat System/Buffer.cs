using System;
using System.Collections.Generic;

/// <summary>
/// Buffer generico, se podria interpretar como una lista de datos limitadas que va ciclando y ejecuta
/// logica extra al añadir o eliminar un elemento.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Buffer<T>
{
    public List<T> buffer;
    public int maxSize = 3;

    public Buffer(int maxSize = 3)
    {
        this.maxSize = maxSize;
        buffer = new List<T>();
    }

    public virtual void Add(T action)
    {
        if (buffer.Count == maxSize)
            buffer.RemoveAt(0);

        buffer.Add(action);
    }

    public virtual void Remove(T action)
    {
        buffer.Remove(action);
    }

    public T GetLastAction()
    {
        return buffer[buffer.Count - 1];
    }

    public virtual void Clear()
    {
        buffer.Clear();
    }
}

/// <summary>
/// Buffer especifico para acciones
/// </summary>
public class ActionBuffer : Buffer<IExecutableAction> 
{
    public ActionBuffer(int maxSize = 3) : base(maxSize) {}

    public override void Add(IExecutableAction action)
    {
        base.Add(action);
        action.OnActionInserted(this);
    }

    public override void Remove(IExecutableAction action)
    {
        base.Remove(action);
        action.OnActionRemoved();
    }

    public override void Clear()
    {
        foreach (var action in buffer)
            action.OnActionRemoved();
        base.Clear();
    }

}
