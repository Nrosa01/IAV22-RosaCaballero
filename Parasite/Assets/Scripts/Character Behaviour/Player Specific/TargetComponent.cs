using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class TargetComponent : CharacterComponent
{
    [SerializeField] Transform target;
    public LayerMask ground;
    public float radius = 2f;
    Camera main;
    Vector3 yOffset = new Vector3(0, 0.05f, 0);
    MeshRenderer rnd;

    public Vector2 GetDir()
    {
        Vector3 dir = Vector2.zero;
        if (target != null)
            dir = target.position - transform.position;
        
        return new Vector2(dir.x, dir.z);
    }

    protected override void Start()
    {
        base.Start();
        main = Camera.main;

        //Cursor.lockState = CursorLockMode.Confined;
        //Cursor.visible = false;

        rnd = target.GetComponent<MeshRenderer>();
    }

    public void SetVisibility(bool visible) => rnd.enabled = visible;

    private void LateUpdate()
    {
        var mousePos = GetMousePosition();
        if (mousePos.success)
            target.position = GetTargetPosClampedByRadius(mousePos.position) + yOffset;
    }

    private Vector3 GetTargetPosClampedByRadius(Vector3 possibleTargetPos)
    {
        Vector3 position = transform.position;
        position.y = possibleTargetPos.y;
        var distance = Vector3.Distance(possibleTargetPos, position);
        if (distance > radius)
        {
            var direction = (possibleTargetPos - position).normalized;
            return transform.position + direction * radius;
        }
        return possibleTargetPos;
    }

    private (bool success, Vector3 position) GetMousePosition()
    {
        Ray ray = main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ground))
            return (success: true, position: hit.point);
        else
            return (success: false, position: Vector3.zero);
    }
}
