using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Modules/BasicSignatureModule", fileName = "BasicSignatureModule")]
public class BasicSignatureModuleSO: ModuleSO
{
    public override Module GetModule(AISensor sensor) => new BasicSignatureModule(sensor);
}
public class BasicSignatureModule : Module
{
    public BasicSignatureModule(AISensor sensor) : base(sensor)
    {
        Debug.Log("BasicSignatureModule");
    }

    public override ModuleStats GetStats()
    {
        return new ModuleStats
        {
            successRate = GetSuccessRate(),
            priority = GetPriority(),
            optimalPos = GetOptimalPosition(),
            type = ModuleType.Signature
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
