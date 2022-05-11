using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private Rigidbody rb;
    int speedId = Animator.StringToHash("Speed");

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        animator.SetFloat(speedId, rb.velocity.magnitude);
    }
}
