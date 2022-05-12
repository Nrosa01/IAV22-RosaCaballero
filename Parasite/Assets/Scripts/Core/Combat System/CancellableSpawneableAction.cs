using Cysharp.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class CancellableSpawneableAction : ExecutableAction, IValidatable
{
    public ICancellableAction spawnableAction;
    [SerializeReference] public ICancellableActionData data;
    Transform transform;
    CharacterBase character;

    public override void Execute()
    {
        //SignalBus<SignalCameraShake>.Fire(new SignalCameraShake(0.2f, 0.1f));
        var go = GameObject.Instantiate(spawnableAction, transform.position, transform.rotation);
        go.DoAction(actionDuration, character, data, this.cancellationToken.Token);
        go.transform.SetParent(transform);
    }

    public override void Init(GameObject self)
    {
        transform = self.transform;
        character = self.GetComponent<CharacterBase>();
    }

    public override ExecutableAction Clone()
    {
        return new CancellableSpawneableAction()
        {
            actionDuration = actionDuration,
            spawnableAction = spawnableAction,
            transform = transform,
            data = this.data,

            //Copy fields from base class, I should use a copy constructor but I'm too lazy
            DurationInBuffer = this.DurationInBuffer,
            PostRecheckTime = this.PostRecheckTime,
            IsExecuting = false,
            HasCooldown = this.HasCooldown,
            delayToNextAction = this.delayToNextAction,
        };
    }

    public void OnValidate()
    {
        if (data == null && spawnableAction != null)
            data = spawnableAction.GetDataType();
    }
}
