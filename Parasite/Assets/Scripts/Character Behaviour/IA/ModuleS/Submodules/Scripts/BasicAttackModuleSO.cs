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
    public float distanceToKeepFromTarget = 2.0f;
    

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
        //return aISensor.transform.position;
        return this.aISensor.GetTarget().position;
    }

    public override bool ShouldExecute => !base.ShouldExecute && DistanceToTarget < distanceToKeepFromTarget;
    float DistanceToTarget => Vector3.Distance(aISensor.transform.position, aISensor.GetTarget().position);


    public override float GetPriority()
    {
        return 0.4f * (base.ShouldExecute ? 0.75f : 1.25f);
    }
    
    public override float GetSuccessRate()
    {
        return 0.8f;
    }

    public override Vector3 GetLookAtPosition()
    {
        return GetOptimalPosition();
    }
}
