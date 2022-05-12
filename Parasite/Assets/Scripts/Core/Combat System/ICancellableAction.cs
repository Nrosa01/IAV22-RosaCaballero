using System;
using System.Threading;
using UnityEngine;

public abstract class ICancellableAction : MonoBehaviour
{
    public abstract void DoAction(float duration, CharacterBase character, ICancellableActionData actionData, CancellationToken token, IExecutableAction actionHolder);
    public abstract ICancellableActionData GetDataType();
}

