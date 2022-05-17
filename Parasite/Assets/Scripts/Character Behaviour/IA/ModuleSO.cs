using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class ModuleSO : ScriptableObject
{
    public abstract Module GetModule(AISensor sensor);
}

public abstract class Module
{
   protected AISensor aISensor;

    public Module(AISensor sensor) 
    {
        aISensor = sensor;
    }

    public void SetSensor(AISensor sensor) => this.aISensor = sensor;
    public abstract ModuleStats GetStats();

    protected abstract float GetSuccessRate();

    protected abstract float GetPriority();
}

public struct ModuleStats
{
    public float successRate;
    public float priority;
    public ModuleType type;
}

public enum ModuleType
{
    Melee,
    Ranged,
    Movement,
    Signature
}