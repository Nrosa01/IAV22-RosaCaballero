using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class BasicDash : CancellableAction
{
    private Rigidbody rb;
    DashData data;

    public override void Init(CharacterBase character, ICancellableActionData actionData, IExecutableAction actionHolder)
    {
        base.Init(character, actionData, actionHolder);
        rb = character.characterInfo.rigidBody;
        transform.SetParent(character.transform);
    }

    protected override void SetActionData(ICancellableActionData data) => this.data = (DashData)data;
    protected override async UniTaskVoid Execute(float duration, CancellationToken token)
    {
        rb.AddForce(character.transform.forward * (this.data.dashForce + rb.velocity.magnitude), ForceMode.Impulse);
    }


    public override ICancellableActionData GetDataType()
    {
        return new DashData();
    }
}