using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

/// <summary>
/// Clase base para acciones spawneable, reusables y cancelables que implementan funcionalidad básica para manejarlos.
/// </summary>
public abstract class CancellableAction : MonoBehaviour
{
    [Header("Important")]

    [SerializeField] bool isReusable = false;

    [Header("Aditional Data")]
    protected IExecutableAction action;
    protected CharacterBase character;

    public Transform owner => character.transform;
    public bool IsReusable => isReusable;

    public void DoAction(float duration, CancellationToken token) => Execute(duration, token).Forget();

    protected abstract UniTaskVoid Execute(float duration, CancellationToken token);

    public virtual void Init(CharacterBase character, ICancellableActionData actionData, IExecutableAction actionHolder)
    {
        this.action = actionHolder;
        this.character = character;
        SetActionData(actionData);
    }
    protected virtual void SetActionData(ICancellableActionData data) { }
    
    public abstract ICancellableActionData GetDataType();

    public virtual CancellableAction GetNewActionInstance(Transform transform) => GameObject.Instantiate(this, transform.position, transform.rotation);
}

