﻿using UnityEngine;

public class CharacterInfo
{
    public CharacterInfo(CharacterBase character)
    {
        this.rigidBody = character.GetComponent<Rigidbody>();
        movementInput = new Vector2(0, 0);
        transform = character.transform;
    }

    Transform transform;
    public Rigidbody rigidBody;
    public Vector2 movementInput;
}