using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollowTargetGroup2 : MonoBehaviour
{
    [SerializeField] private List<Transform> targets;
    [SerializeField] private float smoothTime = 0.5f;
    public Vector3 offset;
    public Vector3 rotation;
    private Vector3 velocity;

    private void LateUpdate()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        Vector3 centerPoint = GetCenterPoint();
        Vector3 newPosition = centerPoint + offset;

        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
        transform.eulerAngles = rotation;
    }


    private float GetGreatestDistance()
    {
        return GetBounds().size.x;
    }

    private Bounds GetBounds()
    {
        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
            bounds.Encapsulate(targets[i].position);

        return bounds;
    }

    Vector3 GetCenterPoint()
    {
        if (targets == null)
            return transform.position;

        return GetBounds().center;
    }

    private void OnDrawGizmos()
    {
        HandleMovement();
    }
}
