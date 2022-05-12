using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitSkills", menuName = "UnitSkills")]
public class UnitSkills_SO : ScriptableObject
{
    [SerializeField] ActionSet_SO meleeActions;
    [SerializeField] ActionSet_SO rangedActions;
    [SerializeField] ActionSet_SO movementActions;
    [SerializeField] ActionSet_SO signatureActions;

    public UnitSkills GetNewInstance() => UnitSkills.GetNewInstance(meleeActions, rangedActions, movementActions, signatureActions);
}

[System.Serializable]
public class UnitSkills
{
    ActionBuffer buffer = new ActionBuffer();

    [SerializeField] SkillSet<CancellableSpawneableAction> meleeActions;
    [SerializeField] SkillSet<CancellableSpawneableAction> rangedActions;
    [SerializeField] SkillSet<CancellableSpawneableAction> movementActions;
    [SerializeField] SkillSet<CancellableSpawneableAction> signatureActions;

    public void Init(GameObject self)
    {
        meleeActions.Init(self);
        rangedActions.Init(self);
        movementActions.Init(self);
        signatureActions.Init(self);
    }

    public void CancelCurrentAction()
    {
        // We dont't know which type of action is the current one
        // Also in the future many actions can be running at the same time
        // So we cancel all of them
        meleeActions.CancelCurrentSkill();
        rangedActions.CancelCurrentSkill();
        movementActions.CancelCurrentSkill();
        signatureActions.CancelCurrentSkill();

        buffer.Clear();
    }

    public bool IsAnySkillExecuting()
    {
        return meleeActions.IsExecuting() || rangedActions.IsExecuting() || movementActions.IsExecuting() || signatureActions.IsExecuting();
    }
        
    public void ExecuteMeleeAction() => TryAddAction(meleeActions.GetCurrentSkill());
    public void ExecuteRangedAction() => TryAddAction(rangedActions.GetCurrentSkill());

    public void ExecuteMovementAction() => TryAddAction(movementActions.GetCurrentSkill());

    public void ExecuteSignatureAction() => TryAddAction(signatureActions.GetCurrentSkill());

    private void TryAddAction(CancellableSpawneableAction action)
    {
        if (action != null && !action.IsExecuting && !action.HasCooldown)
            buffer.Add(action);
    }

    public UnitSkills GetNewInstance()
    {
        UnitSkills skills = new UnitSkills();
        skills.meleeActions = new SkillSet<CancellableSpawneableAction>(meleeActions.skills);
        skills.movementActions = new SkillSet<CancellableSpawneableAction>(movementActions.skills);
        skills.rangedActions = new SkillSet<CancellableSpawneableAction>(rangedActions.skills);
        skills.signatureActions = new SkillSet<CancellableSpawneableAction>(signatureActions.skills);

        return skills;
    }

    public static UnitSkills GetNewInstance(ActionSet_SO meleeActions, ActionSet_SO rangedActions, ActionSet_SO movementActions, ActionSet_SO signatureActions)
    {
        UnitSkills skills = new UnitSkills();
        skills.meleeActions = new SkillSet<CancellableSpawneableAction>(meleeActions != null ? meleeActions.GetActions() : new List<CancellableSpawneableAction>());
        skills.movementActions = new SkillSet<CancellableSpawneableAction>(movementActions != null ? movementActions.GetActions() : new List<CancellableSpawneableAction>());
        skills.rangedActions = new SkillSet<CancellableSpawneableAction>(rangedActions != null ? rangedActions.GetActions() : new List<CancellableSpawneableAction>());
        skills.signatureActions = new SkillSet<CancellableSpawneableAction>(signatureActions != null ? signatureActions.GetActions() : new List<CancellableSpawneableAction>());

        return skills;
    }
}