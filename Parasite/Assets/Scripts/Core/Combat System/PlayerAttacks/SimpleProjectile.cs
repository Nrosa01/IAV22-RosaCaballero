using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SimpleProjectile : CancellableAction, IPoolable
{
    public static MonoPoolGeneric<SimpleProjectile> pool;
    [SerializeReference] SimpleProjectileData data = new SimpleProjectileData();
    Rigidbody rb;

    public override CancellableAction GetNewActionInstance(Transform transform)
    {
        if (pool == null)
            pool = new MonoPoolGeneric<SimpleProjectile>(this);

        return pool.Spawn();
    }

    public override void Init(CharacterBase character, ICancellableActionData actionData, IExecutableAction actionHolder)
    {
        base.Init(character, actionData, actionHolder);
        rb = GetComponent<Rigidbody>();
    }

    protected override void SetActionData(ICancellableActionData data) => this.data = (SimpleProjectileData)data;

    public override ICancellableActionData GetDataType() => new SimpleProjectileData();
    protected override async UniTaskVoid Execute(float duration, CancellationToken token)
    {
        transform.SetParent(character.transform);
        transform.localPosition = data.offset;
        transform.SetParent(null);
        transform.rotation = character.transform.rotation;
        
        while (true)
        {
            await UniTask.Yield(PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
        }
    }

    private void FixedUpdate()
    {
        rb.AccelerateTo(transform.forward * data.speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != character.gameObject && !other.gameObject.CompareTag(StaticTags.InvisibleBounds))
            Despawn();
    }

    [System.Serializable]
    public class SimpleProjectileData : ICancellableActionData
    {
        public float speed;
        public Vector3 offset;
    }

    public void Despawn()
    {
        pool.Despawn(this);
    }

    public void OnSpawn()
    {
    }

    public void OnDespawn()
    {
    }

    public void OnCreated()
    {
    }
}