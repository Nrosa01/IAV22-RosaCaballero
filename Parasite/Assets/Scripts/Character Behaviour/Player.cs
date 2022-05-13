using UnityEditor;
using UnityEngine;

public class Player : CharacterBase
{
    [SerializeField] public InputReader _inputReader = default;
    [SerializeField] UnitSkills_SO alternativeSkills;
    LookAtDir lookAtDir;
    TargetComponent targetComponent;
    bool usingGamepad;

    protected override void Awake()
    {
        base.Awake();

        lookAtDir = GetComponent<LookAtDir>();
        targetComponent = GetComponent<TargetComponent>();
    }


    protected void OnEnable()
    {
        _inputReader.moveEvent += MovePlayer;
        _inputReader.finishMoveEvent += StopCharacterMovement;
        _inputReader.attackMeleeEvent += AttackMeleePlayer;
        _inputReader.attackRangeEvent += AttackRangedPlayer;
        _inputReader.deviceChangedEvent += OnDeviceChange;
        _inputReader.dashEvent += MoveSkill;
    }

    protected void OnDisable()
    {
        _inputReader.moveEvent -= MovePlayer;
        _inputReader.finishMoveEvent -= StopCharacterMovement;
        _inputReader.attackMeleeEvent -= AttackMeleePlayer;
        _inputReader.attackRangeEvent -= AttackRangedPlayer;
        _inputReader.deviceChangedEvent -= OnDeviceChange;
        _inputReader.dashEvent -= MoveSkill;
    }

    public void OnDeviceChange(DeviceType deviceType)
    {
        usingGamepad = deviceType == DeviceType.Gamepad;
        
        if(targetComponent != null)
            targetComponent.SetVisibility(!usingGamepad);
    }

    public void MovePlayer(Vector2 movement)
    {
        if (!IsExecuting || _inputReader.ranged.IsPressed())
            characterInfo.lookAtInput = movement;

            MoveCharacter(movement);
    }

    public void AttackMeleePlayer()
    {
        Rotate(Vector2.zero);
        AttackMelee();
    }

    public void AttackRangedPlayer(Vector2 vec)
    {
        Rotate(vec);
        AttackRanged();
    }

    private void Rotate(Vector2 vec = default)
    {
        if (usingGamepad)
        {
            characterInfo.lookAtInput = vec;
            lookAtDir.RotateTo(vec);
        }
        else
            lookAtDir.RotateTo(targetComponent.GetDir());
    }
}