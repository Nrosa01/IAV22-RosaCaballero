using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public interface IClonable<T>
{
    public T Clone();
}

public interface IExecutableAction
{
    public IExecutableAction priorExecutableAction { get; set; }

    // Esto no es el tiempo que tarda en realizarse la acción, es el tiempo que está en el buffer
    // antes de ser descartada.
    public float DurationInBuffer { get; set; }
    public float TimeLeft { get; set; }
    public float PostRecheckTime { get; set; }
    public bool IsExecuting { get; set; }

    void Execute();

    // Usually here we see if the last action finished executing
    virtual bool CheckCanExecute() => !priorExecutableAction.IsExecuting;

    public event Action ActionExecuted;
    public event Action ActionCancelled;

    //Buffer related stuff
    ActionBuffer Buffer { get; set; }

    public void OnActionInserted(ActionBuffer actionBuffer);

    public void OnActionRemoved();

    UniTaskVoid ActionInBufferDuration(CancellationToken cancellation);

    UniTaskVoid RecheckLoop(CancellationToken cancellation);

    void CancelExecution();

    public void Init(GameObject self);
    
}

public interface IExecutableAction<T> : IExecutableAction, IClonable<T> { }