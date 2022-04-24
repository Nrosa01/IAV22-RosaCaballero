using UnityEngine;

public struct SignalPlayerDies { }

public struct SignalCameraShake
{
    public readonly float shakeStrengh;
    public readonly float shakeDuration;
    public SignalCameraShake(float shakeStrengh, float shakeTime)
    {
        this.shakeStrengh = shakeStrengh;
        this.shakeDuration = shakeTime;
    }
}

[System.Serializable]
public struct CameraShakeData //Inspector asignment only use
{
    [UnityEngine.SerializeField] private float shakeStrengh;
    [UnityEngine.SerializeField] private float shakeDuration;

    public static implicit operator SignalCameraShake(CameraShakeData data) => new SignalCameraShake(data.shakeStrengh, data.shakeDuration);
}

public struct SignalEnemyDies 
{ 
    public Vector2 position; 
}

public struct SignalAllEnemiesDied { }

public struct SignalLevelGenerated { }

public struct SignalRequestNewLevel { }

public struct SignalLevelFinished { }

public struct SignalOnBeat { }