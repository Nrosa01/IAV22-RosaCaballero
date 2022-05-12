using UnityEditor;
using UnityEngine;

public class Player : CharacterBase
{
    [SerializeField] public InputReader _inputReader = default;
    [SerializeField] UnitSkills_SO alternativeSkills;
    
    ContinuosInputAction meleeAttackAction;

    protected override void Awake()
    {
        base.Awake();
        meleeAttackAction = new ContinuosInputAction(AttackMelee);
    }

    protected void OnEnable()
    {
        _inputReader.moveEvent += MoveCharacter;
        _inputReader.finishMoveEvent += StopCharacterMovement;
        _inputReader.attackEvent += meleeAttackAction.Callback;
        _inputReader.dashEvent += ChangeMelee;
    }

    protected void OnDisable()
    {
        _inputReader.moveEvent -= MoveCharacter;
        _inputReader.finishMoveEvent -= StopCharacterMovement;
        _inputReader.attackEvent -= meleeAttackAction.Callback;
        _inputReader.dashEvent -= ChangeMelee;
    }

    void ChangeMelee()
    {
        Debug.Log("Changing skills");
        this.skills.ChangeMeleeSkill(alternativeSkills.MeleeActions);
    }
}