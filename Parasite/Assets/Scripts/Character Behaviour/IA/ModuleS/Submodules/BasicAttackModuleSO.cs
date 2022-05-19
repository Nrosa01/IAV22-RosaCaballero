using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Modules/BasicAttackModule", fileName = "BasicAttackModule")]
public class BasicAttackModuleSO : ModuleSO
{
    public override Module GetModule(AISensor sensor) => new BasicAttackModule(sensor);
}
public class BasicAttackModule : Module
{
    public BasicAttackModule(AISensor sensor) : base(sensor)
    {
        Debug.Log("BasicAttackModule");
    }

    public override ModuleStats GetStats()
    {
        return new ModuleStats
        {
            successRate = GetSuccessRate(),
            priority = GetPriority(),
            optimalPos = GetOptimalPosition(),
            type = ModuleType.Melee
        };
    }

    public override Vector3 GetOptimalPosition()
    {
        return this.aISensor.GetTarget().position;
    }

    public override float GetPriority()
    {
        return 1;
    }

    public override float GetSuccessRate()
    {
        return 1;
    }
}
