using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class BasicSlash : CancellableAction
{
    [SerializeField] private Transform _start, _center, _end;
    [SerializeField] private int _count = 15;
    public GameObject followObject;
    public AnimationCurve curve;
    private Rigidbody rb;
    SlashData data;

    public override void DoAction(float duration, CancellationToken token)
    {
        rb.AddForce(character.transform.forward * (this.data.dashForce + rb.velocity.magnitude), ForceMode.Impulse);
        Slash(duration, token).Forget();
    }

    public override void  Init(CharacterBase character, ICancellableActionData actionData, IExecutableAction actionHolder)
    {
        base.Init(character, actionData, actionHolder);
        rb = character.characterInfo.rigidBody;
        followObject.SetActive(false);
    }

    protected override void SetActionData(ICancellableActionData data) => this.data = (SlashData)data;
    private async UniTaskVoid Slash(float duration, CancellationToken token)
    {
        float time = 0.000001f;
        followObject.SetActive(true);
        //followObject.GetComponent<MeshRenderer>().enabled = true;
        while (time < duration)
        {
            time += Time.deltaTime;
            followObject.transform.position = Vector_Extensions.Slerp(_start.position, _end.position, _center.position, curve.Evaluate(time / duration));
            followObject.transform.LookAt(this.transform);
            await UniTask.Yield(PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
            if (token.IsCancellationRequested)
                break;
        }

        followObject.SetActive(false);
        //Destroy(this.gameObject);
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

    public override ICancellableActionData GetDataType()
    {
        return new SlashData();
    }
}

[System.Serializable]
public class SlashData : ICancellableActionData
{
    public float dashForce = 10.0f;
}