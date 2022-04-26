using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Action/Melee/Basic0", fileName = "MeleeBasic0")]
public class BasicAttackAction : ActionHolder
{
    [System.Serializable]
    class BasicAction : MeleeAction
    {
        [Min(0.05f)]public float actionDuration;
        public GameObject particles;
        Transform transform;

        public override ExecutableAction Clone()
        {
            return new BasicAction()
            {
                actionDuration = actionDuration,
                particles = particles,
                transform = transform,

                //Copy fields from base class
                DurationInBuffer = this.DurationInBuffer,
                IsExecuting = false,
            };
        }

        public override void Execute()
        {
            SignalBus<SignalCameraShake>.Fire(new SignalCameraShake(0.2f, 0.1f));
            Instantiate(particles, transform.position, Quaternion.identity);
            SimulateDelay().Forget();
        }

        public override void Init(GameObject self)
        {
            transform = self.transform;
        }

        async UniTaskVoid SimulateDelay()
        {
            int secondToMilliseconds = 1000;
            await UniTask.Delay((int)(actionDuration * secondToMilliseconds));
            IsExecuting = false;
        }
    }

    [SerializeField]BasicAction action;

    public override ExecutableAction GetAction()
    {
        return action;
    }
}
