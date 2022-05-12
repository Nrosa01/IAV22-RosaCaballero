using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : CharacterBase
{
    [SerializeField] public InputReader _inputReader = default;

    protected void OnEnable()
    {
        _inputReader.moveEvent += MoveCharacter;
        _inputReader.finishMoveEvent += StopCharacterMovement;
        _inputReader.attackEvent += OnAttack;
        _inputReader.dashEvent += MoveSkill;
    }

    protected void OnDisable()
    {
        _inputReader.moveEvent -= MoveCharacter;
        _inputReader.finishMoveEvent -= StopCharacterMovement;
        _inputReader.attackEvent -= OnAttack;
        _inputReader.dashEvent -= MoveSkill;
    }

    private CancellationTokenSource cancellAttackToken = new CancellationTokenSource();
    private void OnAttack(InputActionPhase phase)
    {
        if (phase == InputActionPhase.Performed)
        {
            cancellAttackToken = new CancellationTokenSource();
            TriggerAttack(cancellAttackToken.Token).Forget();
        }
        else if (phase == InputActionPhase.Canceled)
            cancellAttackToken.Cancel();
    }

    private async UniTaskVoid TriggerAttack(CancellationToken cancellation)
    {
        while (true)
        {
            AttackMelee();
            await UniTask.Yield(cancellation);
        }
    }
}
