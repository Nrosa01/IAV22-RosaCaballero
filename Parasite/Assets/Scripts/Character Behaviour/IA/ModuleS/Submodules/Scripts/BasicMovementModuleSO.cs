using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Modules/BasicMovementModule", fileName = "BasicMovementModule")]
public class BasicMovementModuleSO : ModuleSO
{
    public override Module GetModule(AISensor sensor) => new BasicMovementModule(sensor);
}
public class BasicMovementModule : Module
{
    public BasicMovementModule(AISensor sensor) : base(sensor)
    {
        Debug.Log("BasicMovementModule");
    }

    public override ModuleStats GetStats()
    {
        return new ModuleStats
        {
            successRate = GetSuccessRate(),
            priority = GetPriority(),
            optimalPos = GetOptimalPosition(),
            type = ModuleType.Movement
        };
    }

    public override Vector3 GetOptimalPosition()
    {
        Transform closesProjectile = aISensor.GetClosesProjectile();
        if (closesProjectile == null)
            return aISensor.transform.position;
            //closesProjectile = aISensor.GetTarget();

        Vector3 dirToProjectile = (aISensor.transform.position - closesProjectile.position).normalized;
        Vector3 characterPos = aISensor.transform.position;

        Vector3 perpendicularDir = Vector3.Cross(dirToProjectile, Vector3.up).normalized;
        Vector3 perpendicularPos = characterPos + perpendicularDir * 2;

        return perpendicularPos;
    }

    public override float GetPriority()
    {
        return aISensor.GetClosesProjectile() != null ? 1 : 0;
    }

    public override float GetSuccessRate()
    {
        return 1;
    }
}
