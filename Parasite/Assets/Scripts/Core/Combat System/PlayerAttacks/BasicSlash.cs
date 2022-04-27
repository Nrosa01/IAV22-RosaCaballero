using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BasicSlash : MonoBehaviour
{
    [SerializeField] private Transform _start, _center, _end;
    [SerializeField] private int _count = 15;
    public GameObject followObject;
    public float lerpDuration = 0.5f;
    public AnimationCurve curve;
    [SerializeField] public InputReader _inputReader = default;


    private void Start()
    {
        _inputReader.attackEvent += OnAttack;
    }

    private void OnAttack(InputActionPhase phase)
    {
        if (phase == InputActionPhase.Performed)
        {
            StopAllCoroutines();
            StartCoroutine(Slash());
        }
    }

    private IEnumerator Slash()
    {
        float time = 0.000001f;
        followObject.SetActive(true);
        while (time < lerpDuration)
        {
            time += Time.deltaTime;
            followObject.transform.position = Vector_Extensions.Slerp(_start.position, _end.position, _center.position, curve.Evaluate(time / lerpDuration));
            followObject.transform.LookAt(this.transform);
            yield return null;
        }

        followObject.SetActive(false);
    }

    void OnDrawGizmos()
    {

        if (_start == null || _center == null || _end == null)
        {
            return;
        }

        followObject.transform.LookAt(this.transform);

        foreach (var point in EvaluateSlerpPoints(_start.position, _end.position, _center.position, _count))
        {
            Gizmos.DrawSphere(point, 0.1f);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_center.position, 0.2f);
    }

    IEnumerable<Vector3> EvaluateSlerpPoints(Vector3 start, Vector3 end, Vector3 center, int count = 10)
    {
        var f = 1f / count;

        for (var i = 0f; i < 1 + f; i += f)
        {
            yield return Vector_Extensions.Slerp(start, end, center, i);
        }
    }
}
