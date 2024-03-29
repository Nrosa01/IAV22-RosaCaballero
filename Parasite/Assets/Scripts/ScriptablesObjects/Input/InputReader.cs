using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System;
using UnityEngine.InputSystem.LowLevel;
public enum DeviceType { Keyboard, Mouse, Gamepad, Unknown };

[CreateAssetMenu(fileName = "InputReader", menuName = "Game/Input Reader")]

///
/// Clase intermedia para separar el input de la accion que realiza. De esta forma no sabemos que input va a desatar la accion y nos permite desacoplarlo.
///
public class InputReader : ScriptableObject, GameInput.IGameplayActions
{
    //Gameplay
    public event Action<Vector2> moveEvent;
    ContinuosInputAction attackRangedAction;
    public event Action attackMeleeEvent;
    public event Action<Vector2> attackRangeEvent;
    public event Action finishMoveEvent;
    public event Action dashEvent;
    public event Action pauseEvent;
    public event Action<DeviceType> deviceChangedEvent;

    private GameInput gameInput;
    private DeviceType currentDeviceType = DeviceType.Mouse;
    InputDevice currentDevice;
    Vector2 dir;
    public InputAction ranged;

    private void OnEnable()
    {
        attackRangedAction = new ContinuosInputAction(() => attackRangeEvent?.Invoke(dir));
        
        if (gameInput == null)
        {
            gameInput = new GameInput();
            gameInput.Gameplay.SetCallbacks(this);
        }

        ranged = gameInput.FindAction("AttackRanged");


        EnableGameplayInput();
        InputSystem.onEvent += OnInputEvent;
    }

    
    private void OnInputEvent(InputEventPtr args, InputDevice device)
    {
        if (this.currentDevice != device)
        {
            DeviceType deviceType = GetDeviceType(device);

            if (HasChangedDevice(deviceType))
                deviceChangedEvent?.Invoke(deviceType);

            currentDeviceType = deviceType;
            this.currentDevice = device;
        }
    }

    // If currentDeviceType is Mouse and device is Keyboard, return false, return false also is currentDeviceType is Keyboard and device is Mouse
    bool HasChangedDevice(DeviceType newDeviceType)
    {
        if (currentDeviceType == DeviceType.Mouse && newDeviceType == DeviceType.Keyboard)
            return false;
        else if (currentDeviceType == DeviceType.Keyboard && newDeviceType == DeviceType.Mouse)
            return false;
        else
            return true;
    }

    private DeviceType GetDeviceType(InputDevice device)
    {
        if (device is Mouse)
            return DeviceType.Mouse;
        else if (device is Gamepad)
            return DeviceType.Gamepad;
        else if (device is Keyboard)
            return DeviceType.Keyboard;

        return DeviceType.Unknown;
    }

    private void OnDisable()
    {
        Debug.Log("Disabling input");

        DisableAllInput();
        attackRangedAction?.Dispose();
        InputSystem.onEvent -= OnInputEvent;
    }

    public void OnAttackMelee(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed) attackMeleeEvent?.Invoke();
    }
    public void OnAttackRanged(InputAction.CallbackContext context)
    {
        if (this.currentDeviceType == DeviceType.Gamepad)
            dir = context.ReadValue<Vector2>();

        attackRangedAction.Callback(context.phase);
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed) dashEvent?.Invoke();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed) moveEvent?.Invoke(context.ReadValue<Vector2>());
        if (context.phase == InputActionPhase.Canceled) finishMoveEvent?.Invoke();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed) pauseEvent?.Invoke();
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
