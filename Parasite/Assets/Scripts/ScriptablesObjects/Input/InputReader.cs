using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "InputReader", menuName = "Game/Input Reader")]

public class InputReader : ScriptableObject, GameInput.IGameplayActions
{
    //Gameplay
    public event UnityAction<Vector2> moveEvent = delegate { };
    public event UnityAction<InputActionPhase> attackEvent = delegate { };
    public event UnityAction finishMoveEvent = delegate { };
    public event UnityAction dashEvent = delegate { };
    public event UnityAction pauseEvent = delegate { };

    private GameInput gameInput;

    private void OnEnable()
    {
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
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        attackEvent?.Invoke(context.phase);
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
