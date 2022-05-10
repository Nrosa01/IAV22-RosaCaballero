using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] public InputReader _inputReader = default;
    private Vector2 _movementInput; //Initial input coming from the Protagonist script

    public Vector2 movementInput => _movementInput;
    [SerializeField] UnitSkills_SO _skills;
    UnitSkills skills;
    Color defaultColor;
    [SerializeField]Color selectedColor;
    MeshRenderer meshRenderer;


    [HideInInspector] public Rigidbody rb; //Final movement vector, manipulated by the StateMachine actions

    private void Awake()
    {
        if(_skills  == null)
            throw new Exception("Player: Skills not set");
        skills = _skills.GetNewInstance();
        skills.Init(this.gameObject);
        rb = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        defaultColor = meshRenderer.material.color;
    }

    private void OnEnable()
    {
        _inputReader.moveEvent += OnMove;
        _inputReader.finishMoveEvent += OnStop;
        _inputReader.attackEvent += OnAttack;
        _inputReader.dashEvent += OnDash;
        AttackRequested += skills.ExecuteMeleeAction;
    }

    private void OnDisable()
    {
        _inputReader.moveEvent -= OnMove;
        _inputReader.finishMoveEvent -= OnStop;
        _inputReader.attackEvent -= OnAttack;
        _inputReader.dashEvent -= OnDash;
        AttackRequested -= skills.ExecuteMeleeAction;

    }

    private void OnMove(Vector2 movement)
    {
        _movementInput = movement.normalized;
    }

    private void OnStop() => _movementInput = Vector2.zero;

    private void OnDash()
    {
        skills.CancelCurrentAction();
        DashRequested?.Invoke();
    }


    private CancellationTokenSource cancellAttackToken = new CancellationTokenSource();
    private void OnAttack(InputActionPhase phase)
    {
        if (phase == InputActionPhase.Performed)
        {
            meshRenderer.material.color = selectedColor;
            cancellAttackToken = new CancellationTokenSource();
            TriggerAttack(cancellAttackToken.Token).Forget();
        }
        else if (phase == InputActionPhase.Canceled)
        {
            meshRenderer.material.color = defaultColor;
            cancellAttackToken.Cancel();
        }
    }

    private async UniTaskVoid TriggerAttack(CancellationToken cancellation)
    {
        while (true)
        {
            AttackRequested?.Invoke();
            if (cancellation.IsCancellationRequested) break;
            await UniTask.Yield();
        }
    }

    public event Action AttackRequested;
    public event Action DashRequested;
}
