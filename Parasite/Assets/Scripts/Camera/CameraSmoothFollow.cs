using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;

public class CameraSmoothFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Transform mouseInWorld;
    [SerializeField] private float cameraToTargetMoveSpeed = 2f;
    [SerializeField] private float cameraToMouseInfluenceSpeed = 2f;
    [Range(0f,1f)]
    [SerializeField] private float cameraMouseInfluence = 2f;
    [SerializeField] private float maxMouseDistanceFromCenter = 10f;

    float3 smoothedCameraPosition, smoothedMouseInfluence;

    private float3 mouseFromCenter;

    private void Start()
    {
        smoothedCameraPosition = transform.position;
        smoothedMouseInfluence = GetMouseDisplacementFromScreenCenter();
    }
  

    void LateUpdate()
    {
        mouseFromCenter = GetMouseDisplacementFromScreenCenter();
        float3 smoothedMouseDisplacementFromCenter = GenerateSmoothedMousePosition();

        Vector3 targetCameraPosition = target.position * float3(1, 1, 0);

        smoothedCameraPosition = Vector3.Lerp(smoothedCameraPosition, targetCameraPosition, cameraToTargetMoveSpeed * Time.deltaTime);

        transform.position = smoothedCameraPosition + (smoothedMouseDisplacementFromCenter * cameraMouseInfluence);
    }


    private float3 GenerateSmoothedMousePosition()
    {
        smoothedMouseInfluence = Vector3.Lerp(smoothedMouseInfluence, Vector3.ClampMagnitude(mouseFromCenter, maxMouseDistanceFromCenter), cameraToMouseInfluenceSpeed * Time.deltaTime);
        Debug.DrawLine(transform.position, (float3)transform.position + smoothedMouseInfluence, Color.cyan);
        return smoothedMouseInfluence;
    }

    private float3 GetMouseDisplacementFromScreenCenter() => (mouseInWorld.position - transform.position) * float3(1, 1, 0);
}