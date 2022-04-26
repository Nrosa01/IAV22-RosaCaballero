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
        
        public override void Execute()
        {
            Debug.Log("Basic Attack");
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
