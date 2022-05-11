using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereByDistance : MonoBehaviour
{
    public bool isSolid;
    public Transform first;
    public Transform other;
    public float maxRadius;
    public float minRadius;
    public float multiplicador;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        float distance = Vector3.Distance(other.position, this.first.position);
        distance *= multiplicador;

        // Clamp distance
        distance = Mathf.Clamp(distance, minRadius, maxRadius);

        // Draw sphere
        if (isSolid)
            Gizmos.DrawSphere(this.transform.position, distance);
        else
            Gizmos.DrawWireSphere(this.transform.position, distance);

    }
}
