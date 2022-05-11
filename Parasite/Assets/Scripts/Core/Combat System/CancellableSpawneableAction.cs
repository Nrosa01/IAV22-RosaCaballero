using Cysharp.Threading.Tasks;
using UnityEngine;

[System.Serializable] public class CancellableSpawneableAction : ExecutableAction
{
    public ICancellableAction particles;
    Transform transform;

    public override void Execute()
    {
        //SignalBus<SignalCameraShake>.Fire(new SignalCameraShake(0.2f, 0.1f));
        var go = GameObject.Instantiate(particles, transform.position, transform.rotation);
        go.DoAction(actionDuration, this.cancellationToken.Token);
        go.transform.SetParent(transform);
    }

    public override void Init(GameObject self)
    {
        transform = self.transform;
    }

    public override ExecutableAction Clone()
    {
        return new CancellableSpawneableAction()
        {
            actionDuration = actionDuration,
            particles = particles,
            transform = transform,

            //Copy fields from base class, I should use a copy constructor but I'm too lazy
            DurationInBuffer = this.DurationInBuffer,
            PostRecheckTime = this.PostRecheckTime,
            IsExecuting = false,
            HasCooldown = this.HasCooldown,
            delayToNextAction = this.delayToNextAction,
        };
    }
}
