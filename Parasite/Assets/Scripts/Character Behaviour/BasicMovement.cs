using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Este script tambien lo usara el enemigo, pero para prototipar mas rapido por ahora esta hecho para el player
public class BasicMovement : MonoBehaviour
{
    Rigidbody rb;
    public float maxSpeed;
    public float acceleration;
    Camera cam;
    Player p;
    Vector2 previousMovement = Vector2.zero;

    private void Awake()
    {
        p = GetComponent<Player>();
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;
    }

    void Update()
    {
        if (p.movementInput == Vector2.zero) return;
        
        previousMovement = p.movementInput;

        float rotation = Mathf.Atan2(previousMovement.x, previousMovement.y);

        // Create a quaternion (rotation) based on the rotation around the Y axis
        Quaternion q = Quaternion.Euler(0f, rotation * Mathf.Rad2Deg + cam.transform.parent.localEulerAngles.y, 0f);

        // Set the player's rotation to the quaternion
        rb.rotation = q;

    }
    private void FixedUpdate()
    {
        int canMove = p.movementInput == Vector2.zero ? 0 : 1;
        if (rb.velocity.sqrMagnitude < 0.01f && canMove == 0) return;
        rb.AccelerateTo(transform.forward * maxSpeed * canMove, acceleration);
    }
}
