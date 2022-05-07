using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Action/Melee/Basic0", fileName = "MeleeBasic0")]
public class BasicAttackAction : ActionHolder
{
    [System.Serializable]
    class BasicAction : MeleeAction
    {
        [Min(0.05f)]public float actionDuration;
        public ICancellableAction particles;
        Transform transform;

        public override ExecutableAction Clone()
        {
            return new BasicAction()
            {
                actionDuration = actionDuration,
                particles = particles,
                transform = transform,

                //Copy fields from base class, I should use a copy constructor but I'm too lazy
                DurationInBuffer = this.DurationInBuffer,
                PostRecheckTime = this.PostRecheckTime,
                IsExecuting = false,
            };
        }

        public override void Execute()
        {
            //SignalBus<SignalCameraShake>.Fire(new SignalCameraShake(0.2f, 0.1f));
            var go = Instantiate(particles, transform.position, transform.rotation);
            go.DoAction(actionDuration, this.cancellationToken.Token);
            SimulateDelay().Forget();
        }

        public override void Init(GameObject self)
        {
            transform = self.transform;
        }

        async UniTaskVoid SimulateDelay()
        {
            int secondToMilliseconds = 1000;
            await UniTask.Delay((int)(actionDuration * secondToMilliseconds),false, PlayerLoopTiming.Update,cancellationToken.Token);
            IsExecuting = false;
        }
    }

    [SerializeField]BasicAction action;

    public override ExecutableAction GetAction()
    {
        return action.Clone();
    }
}
