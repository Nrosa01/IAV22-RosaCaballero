using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterBase : MonoBehaviour
{
    [SerializeField] UnitSkills_SO _skills;
    [SerializeField] protected Health health;
    protected UnitSkills skills;
    [HideInInspector] public CharacterInfo characterInfo;

    protected void AttackMelee() => skills.ExecuteMeleeAction();
    protected void AttackRanged() => skills.ExecuteRangedAction();
    protected void MoveSkill() => skills.ExecuteMovementAction();
    protected void AttackSignature() => skills.ExecuteSignatureAction();
    protected void MoveCharacter(Vector2 direction) => characterInfo.movementInput = characterInfo.lookAtInput = direction;
    protected void StopCharacterMovement() => characterInfo.movementInput = Vector2.zero;

    public bool IsExecuting => skills.IsAnySkillExecuting();

    protected virtual void Awake()
    {
        characterInfo = new CharacterInfo(this);

        if (_skills == null)
            throw new Exception("CharacterBase: Skills not set");
        skills = _skills.GetNewInstance();
        skills.Init(this.gameObject);
    }
}