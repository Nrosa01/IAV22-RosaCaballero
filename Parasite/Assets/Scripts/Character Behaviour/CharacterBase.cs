using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Clase base que permite controlar un personaje. Necesita recibir skills desde el inspector (aunque también podría recibirlos por addressables).
/// Esta clase define métodos básicos para que una IA o un jugador con Input puedan controlar el personaje abstrayendo el funcionamiento interno.
/// </summary>
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
    protected void MoveCharacter(Vector2 direction) => characterInfo.movementInput = direction;
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