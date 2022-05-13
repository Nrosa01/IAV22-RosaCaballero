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
    Vector2 lastInput;
    Vector3 moveDir;

    protected override void Start()
    {
        base.Start();
        rigidBody = characterInfo.rigidBody;
        cam = Camera.main;
    }

    private void Update() => Move(characterInfo.movementInput);
    
    void Move(Vector2 movement)
    {
        lastInput = movement;
        RotateTowardsDirection(movement);
    }

    [ContextMenu("Test")]
    public void Tesst()
    {
        Vector2 dirUp = new Vector2(0, 1);
        Vector2 forward = new Vector2(-1, 0);

        float forwardToAngle = -Mathf.Atan2(forward.x, forward.y);
        Debug.Log(forwardToAngle);

        Vector2 finalDir = dirUp.Rotated(forwardToAngle);
        Debug.Log("Initial dir: " + dirUp);
        Debug.Log("Final Dir: " + finalDir);
    }

    public Vector3 GetDirRotatedTo(Vector2 dir, Vector2 forward)
    {
        float forwardToAngle = -Mathf.Atan2(forward.x, forward.y);
        Vector2 finalDir = dir.Rotated(forwardToAngle);

        return new Vector3(finalDir.x, 0, finalDir.y);
    }
    
    void RotateTowardsDirection(Vector2 direction)
    {
        if (!ShouldMove)
            return;

        Vector3 camForward = cam.transform.parent.forward;
        Vector2 camForwardWIthoutY = new Vector2(camForward.x, camForward.z);
        
        moveDir = GetDirRotatedTo(direction, camForwardWIthoutY);
        moveDir.Normalize();
    }

    bool ShouldMove => lastInput != Vector2.zero && !character.IsExecuting;

    private void FixedUpdate()
    {
        if (!ShouldMove) 
            return;
        rigidBody.AccelerateTo(moveDir * maxSpeed, acceleration);
    }
}
