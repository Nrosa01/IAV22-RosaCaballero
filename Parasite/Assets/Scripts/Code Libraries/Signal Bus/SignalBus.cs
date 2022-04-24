using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Clase de mensajería utilizada para crear un patrón Observer. <br/>
/// Se basa en el envío de eventos con parámetros englobados en clases de señal.
/// </summary>
/// <typeparam name="T">Clase de señal</typeparam>
public static class SignalBus<T>
{
    static Action<T> action;

    public static IDisposable Subscribe(Action<T> action)
    {
        SignalBus<T>.action += action;
        return new SignalSubscription<T>(action);
    }

    public static void Fire(T t = default) => action?.Invoke(t);

    public static void Unsubscribe(Action<T> action) => SignalBus<T>.action -= action;
}

/// <summary>
/// Usado como <see cref="IDisposable"/> a partir de la suscripción. <br/>
/// Cuando una señal se suscribe se crea una instancia la cual sirve para ser añadida a una lista de IDisposables <br/>
/// y finálmente poder limpiar las suscripciones de la instancia suscrita.
/// </summary>
/// <typeparam name="T">Tipo de señal</typeparam>
public class SignalSubscription<T> : IDisposable
{
    Action<T> bindedAction;

    public SignalSubscription(Action<T> bindedAction) => this.bindedAction = bindedAction;

    public void Dispose() => SignalBus<T>.Unsubscribe(bindedAction);
}

/// <summary> Clase contenedora de lista de disposables. Cuando esta clase se disposa, disposa todos sus elementos.</summary>
public class CompositeDisposable : IDisposable
{
    List<IDisposable> disposables = new List<IDisposable>();

    public void Dispose()
    {
        int iterations = disposables.Count;
        for (int i = 0; i < iterations; i++)
        {
            disposables[i].Dispose();
        }
    }

    public void Add(IDisposable disposable) => disposables.Add(disposable);
}

public static class DisposableExtensions
{
    ///<summary> Añade un disposable al <see cref="CompositeDisposable"/>. </summary>
    public static void AddTo(this IDisposable disposable, CompositeDisposable compositeDisposable) => compositeDisposable.Add(disposable);
}