using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class ContinuosInputAction
{
    Action action;
    bool executing;
    private CancellationTokenSource cancellActionToken = new CancellationTokenSource();

    public ContinuosInputAction(Action action)
    {
        Assert.IsNotNull(action);
        this.action = action;
        executing = false;

        cancellActionToken = new CancellationTokenSource();
        Perform(cancellActionToken.Token).Forget();
    }

    ~ContinuosInputAction()
    {
        cancellActionToken.Cancel();
        cancellActionToken.Dispose();
    }

    public void Callback(InputActionPhase phase)
    {
        if (phase == InputActionPhase.Performed)
            executing = true;
        else if (phase == InputActionPhase.Canceled)
            executing = false;
    }

    private async UniTaskVoid Perform(CancellationToken cancellation)
    {
        while (true)
        {
            if(executing)
                action.Invoke();
            await UniTask.Yield(cancellation);
        }
    }
}
