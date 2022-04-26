using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public interface IExecutableAction
{
    public IExecutableAction priorExecutableAction { get; set; }

    // Esto no es el tiempo que tarda en realizarse la acci�n, es el tiempo que est� en el buffer
    // antes de ser descartada.
    public float DurationInBuffer { get; set; }
    public float TimeLeft { get; set; }
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
}

[System.Serializable]
public abstract class ExecutableAction : IExecutableAction
{
    public abstract void Init(GameObject self);
    public IExecutableAction priorExecutableAction { get; set; }

    // Esto no es el tiempo que tarda en realizarse la acci�n, es el tiempo que est� en el buffer
    // antes de ser descartada.
    [field: SerializeField] public float DurationInBuffer { get; set; }
    public float TimeLeft { get; set; }
    public bool IsExecuting { get; set; }
    public ActionBuffer Buffer { get; set; }

public abstract void Execute();

    // Usually here we see if the last action finished executing
    public virtual bool CheckCanExecute() => !priorExecutableAction.IsExecuting;

    public event Action ActionExecuted;
    public event Action ActionCancelled;

    ~ExecutableAction()
    {
        cancellationToken.Dispose();
    }

    public CancellationTokenSource cancellationToken = new CancellationTokenSource();

    public async UniTaskVoid ActionInBufferDuration(CancellationToken cancellation)
    {
        while (TimeLeft > 0)
        {
            TimeLeft -= Time.deltaTime;
            await UniTask.Yield(PlayerLoopTiming.Update, cancellation);
        }
        Buffer.Remove(this);
    }

    public async UniTaskVoid RecheckLoop(CancellationToken cancellation)
    {
        while (true)
        {
            if (CheckCanExecute())
            {
                Execute();
                IsExecuting = true;
                ActionExecuted?.Invoke();
                Buffer.Remove(this);
                break;
            }
            await UniTask.Yield(PlayerLoopTiming.Update, cancellation);
        }
    }

    public void CancelExecution()
    {
        if (IsExecuting)
        {
            ActionCancelled?.Invoke();
            cancellationToken.Cancel();
            IsExecuting = false;
        }
    }

    public void OnActionRemoved()
    {
        cancellationToken.Cancel();
    }

    public void OnActionInserted(ActionBuffer actionBuffer)
    {
        Buffer = actionBuffer;
        TimeLeft = DurationInBuffer;
        GenericExtensions.CancelAndGenerateNew(ref cancellationToken);
        ActionInBufferDuration(cancellationToken.Token).Forget();
        RecheckLoop(cancellationToken.Token).Forget();
    }

}