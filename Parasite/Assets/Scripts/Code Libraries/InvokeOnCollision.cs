using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InvokeOnCollision : MonoBehaviour
{
    [SerializeField] UnityEvent<Collider> OnTrigger;
    private void OnTriggerEnter(Collider other)
    {
        OnTrigger?.Invoke(other);
    }

    public void TriggerCol(Collider other)
    {
        OnTrigger?.Invoke(other);
    }
}
