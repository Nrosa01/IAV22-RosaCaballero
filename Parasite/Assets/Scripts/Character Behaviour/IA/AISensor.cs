using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISensor : MonoBehaviour
{
    [SerializeField] Transform target;
    Transform nearestProjectile;
    public float checkTime = 0.5f;
    public LayerMask projectilesMask;
    public float projectileDetectionRadius;

    public Transform GetTarget() => target;
    public Transform GetClosesProjectile() => nearestProjectile;

    void Start()
    {
        Check().Forget();
    }

    async UniTaskVoid Check()
    {
        while (true)
        {
            nearestProjectile = GetNearestProjectile();

            await UniTask.Delay((int)(checkTime * 1000), false, default, this.GetCancellationTokenOnDestroy());
        }
    }

    Transform GetNearestProjectile()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, projectileDetectionRadius, projectilesMask);

        if (colliders.Length == 0)
            return null;

        // Get the closest projectile that has SimpleProjectile component
        //Filer colliders list so that only projectiles with SimpleProjectile component are considered
        List<Collider> filteredColliders = new List<Collider>();
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent<SimpleProjectile>(out SimpleProjectile projectile) && projectile?.owner != transform)
                    filteredColliders.Add(collider);
        }

        if (filteredColliders.Count == 0)
            return null;

        Transform nearestProjectile = filteredColliders[0].transform;
        float nearestDistance = Vector3.Distance(transform.position, nearestProjectile.position);

        for (int i = 1; i < filteredColliders.Count; i++)
        {
            float distance = Vector3.Distance(transform.position, filteredColliders[i].transform.position);
            if (distance < nearestDistance)
            {
                nearestProjectile = filteredColliders[i].transform;
                nearestDistance = distance;
            }
        }

        return nearestProjectile;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, projectileDetectionRadius);

        Gizmos.color = Color.cyan;
        if (nearestProjectile != null)
            Gizmos.DrawSphere(nearestProjectile.position, 1f);
    }
}
