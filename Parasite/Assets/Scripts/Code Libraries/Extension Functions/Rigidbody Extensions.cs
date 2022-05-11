using Unity.Mathematics;
using static Unity.Mathematics.math;
using UnityEngine;

public static class Rigidbody_Extensions
{
    public static float2 AccelerateTo2D(this Rigidbody2D rb, Vector2 targetVelocity, float maxAcceleration = float.PositiveInfinity)
    {
        float2 deltaVelocity = targetVelocity - rb.velocity;
        float2 acceleration = deltaVelocity / Time.deltaTime;
        
        if (lengthsq(acceleration) > maxAcceleration * maxAcceleration)
            acceleration = normalize(acceleration) * maxAcceleration;

        rb.AddForce(acceleration, ForceMode2D.Force);
        return acceleration;
    }

    public static float3 AccelerateTo(this Rigidbody rb, Vector3 targetVelocity, float maxAcceleration = float.PositiveInfinity)
    {
        Vector3 deltaVelocity = targetVelocity - rb.velocity;
        float3 acceleration = deltaVelocity / Time.deltaTime;

        if (lengthsq(acceleration) > maxAcceleration * maxAcceleration)
            acceleration = normalize(acceleration) * maxAcceleration;

        rb.AddForce(acceleration, ForceMode.Acceleration);
        return acceleration;
    }
}
