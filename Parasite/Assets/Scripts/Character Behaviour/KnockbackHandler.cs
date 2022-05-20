using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackHandler : CharacterComponent
{
    public float shakeStrngh;
    public float shakeDuration;
    Collider coll;

    public void Awake()
    {
        coll = GetComponent<Collider>();
    }

    public void Knockback(Vector3 dir, float force)
    {
        characterInfo.rigidBody.AddForce(dir * force, ForceMode.Impulse);
        SignalBus<SignalCameraShake>.Fire(new SignalCameraShake(shakeStrngh, shakeDuration));
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out InvokeOnCollision col))
            col.TriggerCol(coll);
    }
}
