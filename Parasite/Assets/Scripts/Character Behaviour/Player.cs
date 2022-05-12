using UnityEditor;
using UnityEngine;

public class Player : CharacterBase
{
    [SerializeField] public InputReader _inputReader = default;
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
        _inputReader.dashEvent += MoveSkill;
    }

    protected void OnDisable()
    {
        _inputReader.moveEvent -= MoveCharacter;
        _inputReader.finishMoveEvent -= StopCharacterMovement;
        _inputReader.attackEvent -= meleeAttackAction.Callback;
        _inputReader.dashEvent -= MoveSkill;
    }
}