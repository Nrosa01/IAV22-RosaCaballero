using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Modules/BasicRangedModule", fileName = "BasicRangedModule")]
public class BasicRangedAttackModuleSO : ModuleSO
{
    public override Module GetModule(AISensor sensor) => new BasicRangedAttackModule(sensor);
}
public class BasicRangedAttackModule : Module
{
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
        return aISensor.transform.position;
        return this.aISensor.GetTarget().position;
    }

    public override float GetPriority()
    {
        return 0.5f;
    }

    public override float GetSuccessRate()
    {
        return 1;
    }
}
