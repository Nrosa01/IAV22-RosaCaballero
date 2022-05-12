using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterBase : MonoBehaviour
{
    [SerializeField] UnitSkills_SO _skills;
    UnitSkills skills;
    [HideInInspector] public CharacterInfo characterInfo;

    public void AttackMelee() => skills.ExecuteMeleeAction();
    public void AttackRanged() => skills.ExecuteRangedAction();
    public void MoveSkill() => skills.ExecuteMovementAction();
    public void AttackSignature() => skills.ExecuteSignatureAction();

    public void MoveCharacter(Vector2 direction) => MoveActionRequested?.Invoke(direction);
    protected void StopCharacterMovement() => MoveActionRequested?.Invoke(Vector2.zero);
    
    public event Action<Vector2> MoveActionRequested;

    protected virtual void Awake()
    {
        characterInfo = new CharacterInfo(this);

        if (_skills == null)
            throw new Exception("CharacterBase: Skills not set");
        skills = _skills.GetNewInstance();
        skills.Init(this.gameObject);
    }
}

public class CharacterInfo
{
    public CharacterInfo(CharacterBase character)
    {
        this.rigidBody = character.GetComponent<Rigidbody>();
    }

    public Rigidbody rigidBody;
}