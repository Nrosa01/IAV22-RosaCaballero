using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

// Interfaz basica para implementar una accion
public interface IExecutableAction
{
    public IExecutableAction priorExecutableAction { get; set; }

    // Esto no es el tiempo que tarda en realizarse la acción, es el tiempo que está en el buffer
    // antes de ser descartada.
    public float DurationInBuffer { get; set; }
    public float TimeLeft { get; set; }
    public float PostRecheckTime { get; set; }
    public bool IsExecuting { get; set; }
    public bool HasCooldown { get; set; }

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

// Esta interfaz fuerza a que las acciones sean clonables y devuelvan el tipo correcto. Necesito clonar los datos de los scriptable objects 
// para no modificarlos en runtime, ya que son persistentes y deben ser de solo lectura.
public interface IExecutableAction<T> : IExecutableAction, IClonable<T> { }