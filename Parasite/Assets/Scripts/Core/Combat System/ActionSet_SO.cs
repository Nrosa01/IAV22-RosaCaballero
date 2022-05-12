using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "Action/ActionSet", fileName = "BasicActionSet")]
public class ActionSet_SO : ScriptableObject
{
    [SerializeField] List<CancellableSpawneableAction> actions;

    public List<CancellableSpawneableAction> GetActions() => actions.Select(x => (CancellableSpawneableAction)x.Clone()).ToList();

    private void OnValidate()
    {
        foreach (var action in actions)
            action.OnValidate();
    }

    private void OnEnable()
    {
        if (actions == null)
            actions = new List<CancellableSpawneableAction>();
    }
}