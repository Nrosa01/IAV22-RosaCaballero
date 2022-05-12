using System;
using System.Threading;
using UnityEngine;

public abstract class CancellableAction : MonoBehaviour
{
    [Header("Important")]

    [SerializeField] bool isReusable = false;

    [Header("Aditional Data")]
    protected IExecutableAction action;
    protected CharacterBase character;

    public bool IsReusable => isReusable;

    public virtual void Init(CharacterBase character, ICancellableActionData actionData, IExecutableAction actionHolder)
    {
        this.action = actionHolder;
        this.character = character;
        SetActionData(actionData);
    }
    protected virtual void SetActionData(ICancellableActionData data) { }
    public abstract void DoAction(float duration, CancellationToken token);
    public abstract ICancellableActionData GetDataType();
}

