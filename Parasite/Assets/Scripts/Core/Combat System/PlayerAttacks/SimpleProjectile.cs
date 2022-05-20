using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// Proyectil simple para testear los ataques a distancia, usa una pool estática de forma que las instancias se reutilizan.
/// Lo ideal sería obtener la pool de un scriptable y que no sea estática pero por cuestiones de tiempo lo he hecho de esta forma. A nivel de
/// funcionalidad es igual, solo es un poco peor a nivel de arquitectura.
/// </summary>
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

        Vector3 collisionDir = (other.transform.position - character.characterInfo.transform.position).normalized;

        if (other.gameObject != character.gameObject && other.gameObject.TryGetComponent(out KnockbackHandler handler))
            handler.Knockback(collisionDir, 10);
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