using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class ContinuosInputAction
{
    Action action;
    private CancellationTokenSource cancellAttackToken = new CancellationTokenSource();

    public ContinuosInputAction(Action action)
    {
        this.action = action;
        Assert.IsNotNull(action);
    }

    public void Callback(InputActionPhase phase)
    {
        if (phase == InputActionPhase.Performed)
        {
            cancellAttackToken = new CancellationTokenSource();
            Perform(cancellAttackToken.Token).Forget();
        }
        else if (phase == InputActionPhase.Canceled)
            cancellAttackToken.Cancel();
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
