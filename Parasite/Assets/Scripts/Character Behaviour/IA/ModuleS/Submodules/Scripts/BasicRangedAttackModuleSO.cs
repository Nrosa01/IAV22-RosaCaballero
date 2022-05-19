using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Modules/BasicRangedModule", fileName = "BasicRangedModule")]
public class BasicRangedAttackModuleSO : ModuleSO
{
    public float distanceToKeepFromTarget = 2.0f;

    public override Module GetModule(AISensor sensor)
    {
        var module = new BasicRangedAttackModule(sensor);
        module.distanceToKeepFromTarget = distanceToKeepFromTarget;
        return module;
    }
}
public class BasicRangedAttackModule : Module
{
    public float distanceToKeepFromTarget = 2.0f;

    public BasicRangedAttackModule(AISensor sensor) : base(sensor)
    {
        Debug.Log("BasicRangedModule");
    }

    public override ModuleStats GetStats()
    {
        return new ModuleStats
        {
            successRate = GetSuccessRate(),
            priority = GetPriority(),
            optimalPos = GetOptimalPosition(),
            type = ModuleType.Ranged
        };
    }

    public override Vector3 GetOptimalPosition()
    {
        Vector3 direction = aISensor.GetTarget().position - aISensor.transform.position;

        return aISensor.transform.position - direction;
    }

    public override bool ShouldExecute => base.ShouldExecute && DistanceToTarget > distanceToKeepFromTarget;

    float DistanceToTarget => Vector3.Distance(aISensor.transform.position, aISensor.GetTarget().position);

    public override float GetPriority()
    {
        return 0.5f * (ShouldExecute ? 0f : 1.0f);
    }

    public override float GetSuccessRate()
    {
        return 1;
    }

    public override Vector3 GetLookAtPosition()
    {
        return this.aISensor.GetTarget().position;
    }
}
