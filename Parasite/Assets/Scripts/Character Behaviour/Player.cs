using UnityEditor;
using UnityEngine;

public class Player : CharacterBase
{
    [SerializeField] public InputReader _inputReader = default;
    [SerializeField] UnitSkills_SO alternativeSkills;
    bool usingGamepad;
    protected void OnEnable()
    {
        _inputReader.moveEvent += MoveCharacter;
        _inputReader.finishMoveEvent += StopCharacterMovement;
        _inputReader.attackMeleeEvent += AttackMelee;
        _inputReader.attackRangeEvent += AttackRangedPlayer;
        _inputReader.deviceChangedEvent += OnDeviceChange;
        _inputReader.dashEvent += MoveSkill;
    }

    protected void OnDisable()
    {
        _inputReader.moveEvent -= MoveCharacter;
        _inputReader.finishMoveEvent -= StopCharacterMovement;
        _inputReader.attackMeleeEvent -= AttackMelee;
        _inputReader.attackRangeEvent -= AttackRangedPlayer;
        _inputReader.deviceChangedEvent -= OnDeviceChange;
        _inputReader.dashEvent -= MoveSkill;
    }

    public void OnDeviceChange(DeviceType deviceType)
    {
        usingGamepad = deviceType == DeviceType.Gamepad;
    }

    public void AttackRangedPlayer(Vector2 vec)
    {
       if(usingGamepad)
            characterInfo.lookAtInput = vec;
       
        AttackRanged();
    }
}