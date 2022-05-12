using Cysharp.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class CancellableSpawneableAction : ExecutableAction, IValidatable
{
    public CancellableAction spawnableAction;
    private CancellableAction _spawnableAction;
    [SerializeReference] public ICancellableActionData data;
    Transform transform;
    CharacterBase character;

    public override void Dispose()
    {
        Debug.Log("Destroying CancellableSpawneableAction");

        if (_spawnableAction != null && spawnableAction.IsReusable)
            GameObject.Destroy(_spawnableAction.gameObject);
    }

    public override void Execute()
    {
        if (!this.spawnableAction.IsReusable)
            SpawnNewAction();

        _spawnableAction.DoAction(actionDuration, cancellationToken.Token);
    }

    private void SpawnNewAction()
    {
        _spawnableAction = GameObject.Instantiate(spawnableAction, transform.position, transform.rotation);
        _spawnableAction.transform.SetParent(transform);
        _spawnableAction.Init(character, data, this);
    }

    public override void Init(GameObject self)
    {
        transform = self.transform;
        character = self.GetComponent<CharacterBase>();

        if (spawnableAction.IsReusable)
            SpawnNewAction();
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
