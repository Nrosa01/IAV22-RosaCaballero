using System.Threading;
using UnityEngine;

public abstract class ICancellableAction : MonoBehaviour
{
    public abstract void DoAction(float duration, CancellationToken token);
}

