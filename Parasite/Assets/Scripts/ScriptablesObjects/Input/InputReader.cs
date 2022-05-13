using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System;
using UnityEngine.InputSystem.LowLevel;
public enum DeviceType { Keyboard, Mouse, Gamepad, Unknown };

[CreateAssetMenu(fileName = "InputReader", menuName = "Game/Input Reader")]

public class InputReader : ScriptableObject, GameInput.IGameplayActions
{
    //Gameplay
    public event Action<Vector2> moveEvent;
    ContinuosInputAction attackAction;
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
        attackAction = new ContinuosInputAction(() => attackMeleeEvent?.Invoke());
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
        DisableAllInput();
        attackAction?.Dispose();
        attackRangedAction?.Dispose();
        InputSystem.onEvent -= OnInputEvent;
    }

    public void OnAttackMelee(InputAction.CallbackContext context)
    {
        attackAction.Callback(context.phase);
    }
    public void OnAttackRanged(InputAction.CallbackContext context)
    {
        if (this.currentDeviceType == DeviceType.Gamepad)
            dir = context.ReadValue<Vector2>();

        attackRangedAction.Callback(context.phase);
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (dashEvent != null && context.phase == InputActionPhase.Performed) dashEvent.Invoke();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (moveEvent != null && context.phase == InputActionPhase.Performed) moveEvent?.Invoke(context.ReadValue<Vector2>());
        if (finishMoveEvent != null && context.phase == InputActionPhase.Canceled) finishMoveEvent?.Invoke();
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
