using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System;

[CreateAssetMenu(fileName = "InputReader", menuName = "Game/Input Reader")]

public class InputReader : ScriptableObject, GameInput.IGameplayActions
{
    //Gameplay
    public event Action<Vector2> moveEvent;
    ContinuosInputAction attackAction;
    ContinuosInputAction attackRangedAction;
    public event Action attackMeleeEvent;
    public event Action attackRangeEvent;
    public event Action finishMoveEvent;
    public event Action dashEvent;
    public event Action pauseEvent;

    private GameInput gameInput;

    private void OnEnable()
    {
        attackAction = new ContinuosInputAction(() => attackMeleeEvent?.Invoke());
        attackRangedAction = new ContinuosInputAction(() => attackRangeEvent?.Invoke());

        if (gameInput == null)
        {
            gameInput = new GameInput();
            gameInput.Gameplay.SetCallbacks(this);
        }
        EnableGameplayInput();
    }

    private void OnDisable()
    {
        DisableAllInput();
        attackAction?.Dispose();
    }

    public void OnAttackMelee(InputAction.CallbackContext context)
    {
        attackAction.Callback(context.phase);
    }
    public void OnAttackRanged(InputAction.CallbackContext context)
    {
        attackRangedAction.Callback(context.phase);
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (dashEvent != null && context.phase == InputActionPhase.Performed) dashEvent.Invoke();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if(moveEvent != null && context.phase == InputActionPhase.Performed) moveEvent?.Invoke(context.ReadValue<Vector2>());
        if(finishMoveEvent != null && context.phase == InputActionPhase.Canceled) finishMoveEvent?.Invoke();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (pauseEvent != null && context.phase == InputActionPhase.Performed) pauseEvent.Invoke();
    }

    public void EnableGameplayInput()
    {
        gameInput.Gameplay.Enable();
    }

    public void DisableAllInput()
    {
        gameInput.Gameplay.Disable();
    }
}
