using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class ModuleSO : ScriptableObject
{
    public int maxRepetitions = 5;
    public float secondsToReset = 3;

    public abstract Module GetModule(AISensor sensor);

    public Module GetModule2(AISensor sensor)
    {
        Module mod = GetModule(sensor);
        mod.SetTimeConfig(secondsToReset, maxRepetitions);
        return mod;
    }
}

public abstract class Module
{
    public int maxRepetitions = 5;
    int repetitions = 0;
    public float secondsToReset = 3;

    protected AISensor aISensor;

    public void SetTimeConfig(float secondsToReset, int maxRepetitions)
    {
        this.secondsToReset = secondsToReset;
        this.maxRepetitions = maxRepetitions;
    }

    public Module(AISensor sensor, int maxRepetitions = 1000000, float secondsToReset = 0)
    {
        this.maxRepetitions = maxRepetitions;
        this.secondsToReset = secondsToReset;
        this.repetitions = 0;
        aISensor = sensor;
    }

    public void SetSensor(AISensor sensor) => this.aISensor = sensor;
    public abstract ModuleStats GetStats();

    public abstract float GetSuccessRate();

    public abstract float GetPriority();

    public abstract Vector3 GetOptimalPosition();
    public abstract Vector3 GetLookAtPosition();

    public virtual bool ShouldExecute => repetitions >= maxRepetitions;

    public virtual void OnExecuted()
    {
        repetitions++;

        if (repetitions == maxRepetitions)
            Cooldown().Forget();

    }

    async UniTaskVoid Cooldown()
    {
        await UniTask.Delay((int)(secondsToReset * 1000), false, default, aISensor.GetCancellationTokenOnDestroy());
        repetitions = 0;
    }
}

public struct ModuleStats
{
    public float successRate;
    public float priority;
    public Vector3 optimalPos;
    public Vector3 lookAt;
    public ModuleType type;
}

public enum ModuleType
{
    Melee,
    Ranged,
    Movement,
    Signature
}