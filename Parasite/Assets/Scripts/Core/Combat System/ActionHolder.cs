using UnityEngine;

public abstract class ActionHolder : ScriptableObject
{
    public abstract ExecutableAction GetAction();
}
