using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Este script tambien lo usara el enemigo, pero para prototipar mas rapido por ahora esta hecho para el player
public class LookAtDir : CharacterComponent
{
    Rigidbody rigidBody;
    Camera cam;
    Vector2 directionInput;

    protected override void Start()
    {
        base.Start();
        rigidBody = characterInfo.rigidBody;
        cam = Camera.main;
    }

    private void Update() => RotateTo(characterInfo.lookAtInput);

    public void RotateTo(Vector2 direction)
    {
        directionInput = direction;
        RotateTowardsDirection(direction);
    }

    void RotateTowardsDirection(Vector2 direction)
    {
        if (!ShouldMove)
            return;

        float rotation = Mathf.Atan2(direction.x, direction.y);
        rigidBody.rotation = Quaternion.Euler(0f, rotation * Mathf.Rad2Deg + cam.transform.parent.localEulerAngles.y, 0f); ;
    }

    bool ShouldMove => directionInput != Vector2.zero && !character.IsExecuting;
}
