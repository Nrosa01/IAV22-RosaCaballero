using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Este script tambien lo usara el enemigo, pero para prototipar mas rapido por ahora esta hecho para el player
public class BasicMovement : CharacterComponent
{
    Rigidbody rigidBody;
    public float maxSpeed;
    public float acceleration;
    Camera cam;
    Vector2 movementInput;

    protected override void Awake()
    {
        base.Awake();
        rigidBody = characterInfo.rigidBody;
        cam = Camera.main;
        character.MoveActionRequested += OnMoveEvent;
    }

    private void OnDestroy()
    {
        character.MoveActionRequested -= RotateTowardsDirection;
    }

    void OnMoveEvent(Vector2 movement)
    {
        movementInput = movement;
        RotateTowardsDirection(movement);
    }

    void RotateTowardsDirection(Vector2 direction)
    {
        if (direction == Vector2.zero)
            return;
        
        float rotation = Mathf.Atan2(direction.x, direction.y);
        rigidBody.rotation = Quaternion.Euler(0f, rotation * Mathf.Rad2Deg + cam.transform.parent.localEulerAngles.y, 0f); ;
    }

    private void FixedUpdate()
    {
        if (movementInput == Vector2.zero) 
            return;
        rigidBody.AccelerateTo(transform.forward * maxSpeed, acceleration);
    }
}
