using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] public InputReader _inputReader = default;
    
    [SerializeField] UnitSkills_SO _skills;
    UnitSkills skills;

    [HideInInspector] public Rigidbody rb; //Final movement vector, manipulated by the StateMachine actions
    public Vector2 movementInput => _movementInput;
    private Vector2 _movementInput; //Initial input coming from the Protagonist script

    private void Awake()
    {
        if(_skills  == null)
            throw new Exception("Player: Skills not set");
        skills = _skills.GetNewInstance();
        skills.Init(this.gameObject);
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        _inputReader.moveEvent += OnMove;
        _inputReader.finishMoveEvent += OnStop;
        _inputReader.attackEvent += OnAttack;
        _inputReader.dashEvent += OnMoveAction;
        AttackRequested += skills.ExecuteMeleeAction;
    }

    private void OnDisable()
    {
        _inputReader.moveEvent -= OnMove;
        _inputReader.finishMoveEvent -= OnStop;
        _inputReader.attackEvent -= OnAttack;
        _inputReader.dashEvent -= OnMoveAction;
        AttackRequested -= skills.ExecuteMeleeAction;

    }

    private void OnMove(Vector2 movement)
    {
        _movementInput = movement;
    }

    private void OnStop() => _movementInput = Vector2.zero;

    private void OnMoveAction()
    {
        skills.CancelCurrentAction();
        MoveActionRequested?.Invoke();
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
            AttackRequested?.Invoke();
            await UniTask.Yield(cancellation);
        }
    }

    public event Action AttackRequested;
    public event Action MoveActionRequested;
}
