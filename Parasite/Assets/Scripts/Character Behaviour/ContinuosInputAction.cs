using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class ContinuosInputAction
{
    Action action;
    private CancellationTokenSource cancellActionToken = new CancellationTokenSource();

    public ContinuosInputAction(Action action)
    {
        this.action = action;
        Assert.IsNotNull(action);
    }

    public void Callback(InputActionPhase phase)
    {
        if (phase == InputActionPhase.Performed)
        {
            cancellActionToken = new CancellationTokenSource();
            Perform(cancellActionToken.Token).Forget();
        }
        else if (phase == InputActionPhase.Canceled)
            cancellActionToken.Cancel();
    }

    private async UniTaskVoid Perform(CancellationToken cancellation)
    {
        while (true)
        {
            action.Invoke();
            await UniTask.Yield(cancellation);
        }
    }
}
