using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitchAddonMultitarget : MonoBehaviour
{

    public Transform target;
    public Transform start;
    public Transform end;
    public float minPitch, maxPitch;
    CameraMultiTarget multiTarget;
    float maxDistance;

    void Start()
    {
        multiTarget = GetComponent<CameraMultiTarget>();
        maxDistance = Vector3.Distance(start.position, end.position);
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePitch();
    }

    private void UpdatePitch()
    {
        multiTarget.Pitch = Mathf.Lerp(minPitch, maxPitch, GetDistance() / maxDistance);
    }

    private float GetDistance()
    {
        return Vector3.Distance(target.position, start.position);
    }

    private void OnDrawGizmos()
    {
        if (enabled && !Application.isPlaying)
        {
            multiTarget = GetComponent<CameraMultiTarget>();
            maxDistance = Vector3.Distance(start.position, end.position);
            UpdatePitch();
        }
    }
}
