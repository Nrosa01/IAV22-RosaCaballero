using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

[System.Serializable]
public abstract class ExecutableAction : IExecutableAction<ExecutableAction>, IDisposable
{
    public ExecutableAction() { }
    ~ExecutableAction()
    {
        cancellationToken.Dispose();
    }

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
    [Tooltip("Action duration in seconds")][Min(0.05f)] public float actionDuration;
    [Tooltip("Sort of cooldown, time needed before you can execute next action")][Min(0.0f)] public float delayToNextAction;
    public bool IsExecuting { get; set; } = false;
    public bool HasCooldown { get; set; }
    
    public ActionBuffer Buffer { get; set; }

    public abstract void Execute();

    // Usually here we see if the last action finished executing
    public virtual bool CheckCanExecute() => !priorExecutableAction.IsExecuting && !priorExecutableAction.HasCooldown && Buffer.GetLastAction() == this;

    public event Action ActionExecuted;
    public event Action ActionCancelled;

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
                IsExecuting = actionDuration > 0; //If the action duration is 0, it means it is a one shot action
                HasCooldown = true;
                ActionExecuted?.Invoke();
                Buffer.Remove(this);
                SimulateDelay().Forget();
                break;
            }
            await UniTask.Yield(PlayerLoopTiming.Update, cancellation);
            if (cancellation.IsCancellationRequested)
                break;
        }

        //This is executed when the action is executed
        //Debug.Log("Action executed");
    }

    async UniTaskVoid SimulateDelay()
    {
        int secondToMilliseconds = 1000;
        await UniTask.Delay((int)(actionDuration * secondToMilliseconds), false, PlayerLoopTiming.Update, cancellationToken.Token);
        IsExecuting = false;
        await UniTask.Delay((int)(delayToNextAction * secondToMilliseconds), false, PlayerLoopTiming.Update, cancellationToken.Token);
        HasCooldown = false;
    }

    public void CancelExecution()
    {
        if (IsExecuting)
        {
            ActionCancelled?.Invoke();
            cancellationToken.Cancel();
            IsExecuting = false;
            HasCooldown = false;
            Buffer.Clear(); // Limpiar buffer, si se cancela una acción se interrumpen todas las demás
        }
    }

    public void OnActionRemoved()
    {
        if (!IsExecuting && !HasCooldown)
            cancellationToken.Cancel();
    }

    public void OnActionInserted(ActionBuffer actionBuffer)
    {
        if (IsExecuting || HasCooldown)
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

    public abstract void Dispose();
}