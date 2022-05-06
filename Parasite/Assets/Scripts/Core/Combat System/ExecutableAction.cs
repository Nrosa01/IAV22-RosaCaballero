using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

[System.Serializable]
public abstract class ExecutableAction : IExecutableAction<ExecutableAction>
{
    public ExecutableAction() { }

    public ExecutableAction(ExecutableAction actionToCopy)
    {
        this.DurationInBuffer = actionToCopy.DurationInBuffer;
        this.TimeLeft = 0;
        this.IsExecuting = false;
    }

    public abstract void Init(GameObject self);
    public IExecutableAction priorExecutableAction { get; set; }

    [Tooltip("Time in seconds that the action is in the buffer before being discarded.")]
    [field: SerializeField] public float DurationInBuffer { get; set; }
    public float TimeLeft { get; set; }
    [Tooltip("Time in seconds since the action finish it will be buffered before being discarded. Once this time is reached, current " +
        "action will be reset to 0")]
    [field: SerializeField] public float PostRecheckTime { get; set; } = 0.1f;
    public bool IsExecuting { get; set; } = false;
    public ActionBuffer Buffer { get; set; }

    public abstract void Execute();

    // Usually here we see if the last action finished executing
    public virtual bool CheckCanExecute() => !priorExecutableAction.IsExecuting && Buffer.GetLastAction() == this;

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
            // Even if we can execute, we have to check we are the last action in the buffer (last = more priority)
            if (CheckCanExecute())
            {
                // There might be another of the same type in the buffer waiting, so we force it to stop
                GenericExtensions.CancelAndGenerateNew(ref cancellationToken);
                Execute();
                IsExecuting = true;
                ActionExecuted?.Invoke();
                Buffer.Remove(this);
                break;
            }
            await UniTask.Yield(PlayerLoopTiming.Update, cancellation);
            if (cancellation.IsCancellationRequested)
                break;
        }

        //This is executed when the action is executed
        //Debug.Log("Action executed");
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
        if (!IsExecuting)
            cancellationToken.Cancel();
    }

    public void OnActionInserted(ActionBuffer actionBuffer)
    {
        if (IsExecuting)
        {
            actionBuffer.Remove(this);
            return;
        }

        Buffer = actionBuffer;
        TimeLeft = DurationInBuffer;
        GenericExtensions.CancelAndGenerateNew(ref cancellationToken);
        ActionInBufferDuration(cancellationToken.Token).Forget();
        RecheckLoop(cancellationToken.Token).Forget();
    }

    public abstract ExecutableAction Clone();
}