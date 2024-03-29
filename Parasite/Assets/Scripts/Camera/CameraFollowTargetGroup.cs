using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowTargetGroup : MonoBehaviour
{
    public bool isSolid;
    public Transform first;
    public Transform other;
    public Transform target;
    public float maxRadius;
    public float minRadius;
    public float multiplicador;
    public Transform dynamicObjects;

    public Vector3 GetAverage()
    {
        return (first.position + other.position) / 2;
    }

    private void LateUpdate()
    {
        FollowGroup();
    }

    private void FollowGroup()
    {
        float distance = Vector3.Distance(other.position, this.first.position);
        distance *= multiplicador;

        // Clamp distance
        float unclampedDistance = Mathf.Clamp(distance, minRadius, distance);
        distance = Mathf.Clamp(distance, minRadius, maxRadius);

        transform.LookAt(target);
        Vector3 average = GetAverage();
        Vector3 closestz = first.transform.position.z > other.transform.position.z ? other.position : first.position;
        this.transform.position = new Vector3(average.x, average.y + distance, average.z - (target.forward * distance).z);
    }

    private void OnDrawGizmos()
    {
        if (!enabled) return;
        Gizmos.color = Color.red;
        float distance = Vector3.Distance(other.position, this.first.position);
        distance *= multiplicador;

        // Clamp distance
        distance = Mathf.Clamp(distance, minRadius, maxRadius);

        // Draw sphere
        if (isSolid)
            Gizmos.DrawSphere(this.first.position, distance);
        else
            Gizmos.DrawWireSphere(this.first.position, distance);

        Gizmos.DrawSphere(GetAverage(), 1.0f);

        FollowGroup();
    }
}
