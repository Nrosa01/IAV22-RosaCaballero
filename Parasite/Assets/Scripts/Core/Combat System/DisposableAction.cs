//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class DisposableAction<T> : IDisposable
//{
//    Action<T> action;
//    CompositeDisposable disposables = new CompositeDisposable();

//    public void Add(Action<T> action)
//    {
//        action += action;
//        disposables.Add(new SignalSubscription<T>(action));
//    }

//    public void Dispose()
//    {
//        disposables.Dispose();
//    }

//    public void Invoke(T t = default) => action?.Invoke(t);
//}

//public class DisposableAction : IDisposable
//{
//    Action action;
//    CompositeDisposable disposables = new CompositeDisposable();

//    public void Add(Action action)
//    {
//        action += action;
//        disposables.Add(new SignalSubscription(action));
//    }

//    public void Dispose()
//    {
//        disposables.Dispose();
//    }

//    public void Invoke() => action?.Invoke();
//}

//public class SignalSubscription: IDisposable
//{
//    Action bindedAction;

//    public SignalSubscription(Action bindedAction) => this.bindedAction = bindedAction;

//    public void Dispose() => SignalBus.Unsubscribe(bindedAction);
//}
