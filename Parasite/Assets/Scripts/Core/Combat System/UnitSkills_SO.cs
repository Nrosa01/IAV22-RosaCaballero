using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu(fileName = "UnitSkills", menuName = "UnitSkills")]
public class UnitSkills_SO : ScriptableObject
{
    [SerializeField] ActionSet_SO meleeActions;
    [SerializeField] ActionSet_SO rangedActions;
    [SerializeField] ActionSet_SO movementActions;
    [SerializeField] ActionSet_SO signatureActions;

    public ActionSet_SO MeleeActions { get { return meleeActions; } }
    public ActionSet_SO RangedActions { get { return rangedActions; } }
    public ActionSet_SO MovementActions { get { return movementActions; } }
    public ActionSet_SO SignatureActions { get { return signatureActions; } }

    public static List<CancellableSpawneableAction> ParseAction(ActionSet_SO actionSet) => actionSet != null ? actionSet.GetActions() : new List<CancellableSpawneableAction>();
    public UnitSkills GetNewInstance() => UnitSkills.GetNewInstance(meleeActions, rangedActions, movementActions, signatureActions);
}

/// <summary>
/// Clase que gestiona las habilidades de una unidad, permite ejecutar las acciones, cancelarlas, comprobar si hay alguna ejectuándose...
/// Internamente implementa un buffer compartido para todos los tipos de acciones y tiene una referencia al Gameobject que la usa
/// </summary>
[System.Serializable]
public class UnitSkills
{
    ActionBuffer buffer = new ActionBuffer();
    GameObject unit;

    [SerializeField] SkillSet<CancellableSpawneableAction> meleeActions;
    [SerializeField] SkillSet<CancellableSpawneableAction> rangedActions;
    [SerializeField] SkillSet<CancellableSpawneableAction> movementActions;
    [SerializeField] SkillSet<CancellableSpawneableAction> signatureActions;

    public void Init(GameObject self)
    {
        this.unit = self;
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

    public bool IsAnySkillExecuting() => meleeActions.IsExecuting() || rangedActions.IsExecuting() || movementActions.IsExecuting() || signatureActions.IsExecuting();
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

    public void ChangeMeleeSkill(ActionSet_SO newMeleeActions)
    {
        Assert.IsNotNull(newMeleeActions);
        meleeActions.Dispose();
        meleeActions = new SkillSet<CancellableSpawneableAction>(newMeleeActions.GetActions());
        meleeActions.Init(unit);
    }

    public void ChangeRangedSkill(ActionSet_SO newRangedActions)
    {
        Assert.IsNotNull(newRangedActions);
        rangedActions.Dispose();
        rangedActions = new SkillSet<CancellableSpawneableAction>(newRangedActions.GetActions());
        rangedActions.Init(unit);
    }

    public void ChangeMovementSkill(ActionSet_SO newMovementActions)
    {
        Assert.IsNotNull(newMovementActions);
        movementActions.Dispose();
        movementActions = new SkillSet<CancellableSpawneableAction>(newMovementActions.GetActions());
        movementActions.Init(unit);
    }

    public void ChangeSignatureSkill(ActionSet_SO newSignatureActions)
    {
        Assert.IsNotNull(newSignatureActions);
        signatureActions.Dispose();
        signatureActions = new SkillSet<CancellableSpawneableAction>(newSignatureActions.GetActions());
        signatureActions.Init(unit);
    }

    public static UnitSkills GetNewInstance(ActionSet_SO meleeActions, ActionSet_SO rangedActions, ActionSet_SO movementActions, ActionSet_SO signatureActions)
    {
        UnitSkills skills = new UnitSkills();
        skills.meleeActions = new SkillSet<CancellableSpawneableAction>(UnitSkills_SO.ParseAction(meleeActions));
        skills.movementActions = new SkillSet<CancellableSpawneableAction>(UnitSkills_SO.ParseAction(movementActions));
        skills.rangedActions = new SkillSet<CancellableSpawneableAction>(UnitSkills_SO.ParseAction(rangedActions));
        skills.signatureActions = new SkillSet<CancellableSpawneableAction>(UnitSkills_SO.ParseAction(signatureActions));

        return skills;
    }
}