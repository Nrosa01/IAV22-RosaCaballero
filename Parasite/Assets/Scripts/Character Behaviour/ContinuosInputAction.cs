using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

/// <summary>
/// Clase para extender la funcionalidad de Input De Unity. Permite simular el equivalente a "GetButton" a nivel de input.
/// Esta clase recibe una accion y tiene un callback para registrar en el middleware InputReader, cuando se pulsa una tecla,
/// se ejecuta la acción que pasamos al crear la clase y no parará de ejecutarse hasta que se deje de pulsar.
/// </summary>
public class ContinuosInputAction : IDisposable
{
    Action action;
    bool executing, isDisposed;

    public ContinuosInputAction(Action action)
    {
        this.action = action;
        executing = false;
        isDisposed = false;

        Perform().Forget();
    }

    public void Callback(InputActionPhase phase)
    {
        if (phase == InputActionPhase.Performed)
            executing = true;
        else if (phase == InputActionPhase.Canceled)
            executing = false;
    }

    public void Dispose()
    {
        isDisposed = true;
    }

    private async UniTaskVoid Perform()
    {
        while (!isDisposed)
        {
            if (executing)
                action.Invoke();
            await UniTask.Yield();
        }
    }
}
