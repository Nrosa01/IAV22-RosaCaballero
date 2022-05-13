using UnityEditor;
using UnityEngine;

public class Player : CharacterBase
{
    [SerializeField] public InputReader _inputReader = default;
    [SerializeField] UnitSkills_SO alternativeSkills;
    protected void OnEnable()
    {
        _inputReader.moveEvent += MoveCharacter;
        _inputReader.finishMoveEvent += StopCharacterMovement;
        _inputReader.attackMeleeEvent += AttackMelee;
        _inputReader.dashEvent += MoveSkill;
    }

    protected void OnDisable()
    {
        _inputReader.moveEvent -= MoveCharacter;
        _inputReader.finishMoveEvent -= StopCharacterMovement;
        _inputReader.attackMeleeEvent -= AttackMelee;
        _inputReader.dashEvent -= MoveSkill;
    }
}