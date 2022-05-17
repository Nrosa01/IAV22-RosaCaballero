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
        throw new System.NotImplementedException();
    }

    protected override float GetPriority()
    {
        throw new System.NotImplementedException();
    }

    protected override float GetSuccessRate()
    {
        throw new System.NotImplementedException();
    }
}
