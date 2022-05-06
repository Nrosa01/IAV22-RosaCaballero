using UnityEngine;

[CreateAssetMenu(fileName = "UnitSkills", menuName = "UnitSkills")]
public class UnitSkills : ScriptableObject
{
    ActionBuffer buffer = new ActionBuffer();
    bool isCloned = false;

    [SerializeField] SkillSet<MeleeAction> meleeActions;
    [SerializeField] SkillSet<RangedAction> rangedActions;
    [SerializeField] SkillSet<MovementAction> movementActions;
    [SerializeField] SkillSet<SignatureAction> signatureActions;

    public void Init(GameObject self)
    {
        if (!isCloned)
            throw new System.Exception("UnitSkills must be cloned before being initialized");

        meleeActions.Init(self);
        rangedActions.Init(self);
        movementActions.Init(self);
        signatureActions.Init(self);
    }

    public void CancelCurrentAction()
    {
        // We dont't know which type of action is the current one
        // So we cancel all of them
        meleeActions.CancelCurrentSkill();
        rangedActions.CancelCurrentSkill();
        movementActions.CancelCurrentSkill();
        signatureActions.CancelCurrentSkill();

        buffer.Clear();
    }

    public void ExecuteMeleeAction()
    {
        buffer.Add(meleeActions.GetCurrentSkill());
    }

    public void ExecuteRangedAction()
    {
        buffer.Add(rangedActions.GetCurrentSkill());
    }

    public void ExecuteMovementAction()
    {
        buffer.Add(movementActions.GetCurrentSkill());
    }

    public void ExecuteSignatureAction()
    {
        buffer.Add(signatureActions.GetCurrentSkill());
    }

    public UnitSkills GetNewInstance()
    {
        UnitSkills skills = this.Clone();
        skills.meleeActions = new SkillSet<MeleeAction>(meleeActions.skills);
        skills.movementActions = new SkillSet<MovementAction>(movementActions.skills);
        skills.rangedActions = new SkillSet<RangedAction>(rangedActions.skills);
        skills.signatureActions = new SkillSet<SignatureAction>(signatureActions.skills);
        skills.isCloned = true;

        return skills;
    }
}