using UnityEngine;

[CreateAssetMenu(fileName = "UnitSkills", menuName = "UnitSkills")]
public class UnitSkills : ScriptableObject
{
    ActionBuffer buffer = new ActionBuffer();

   [SerializeField] SkillSet<MeleeAction> meleeActions;
   [SerializeField] SkillSet<RangedAction> rangedActions;
   [SerializeField] SkillSet<MovementAction> movementActions;
   [SerializeField] SkillSet<SignatureAction> signatureActions;

    public void Init(GameObject self)
    {
        Debug.Log("UnitSkills.OnEnable");
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
        UnitSkills skills = ScriptableObject.CreateInstance<UnitSkills>();
        skills.movementActions = movementActions.GetNewInstance();
        skills.rangedActions = rangedActions.GetNewInstance();
        skills.meleeActions = meleeActions.GetNewInstance();
        skills.signatureActions = signatureActions.GetNewInstance();

        return skills;
    }
}