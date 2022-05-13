using UnityEngine;

public class CharacterInfo
{
    public CharacterInfo(CharacterBase character)
    {
        this.rigidBody = character.GetComponent<Rigidbody>();
        movementInput = new Vector2(0, 0);
        lookAtInput = new Vector2(0, 0);
        transform = character.transform;
    }

    public Transform transform;
    //public Vector3 aimDirection;
    public Rigidbody rigidBody;
    public Vector2 movementInput;
    public Vector2 lookAtInput;
}