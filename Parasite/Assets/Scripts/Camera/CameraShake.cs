using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private CompositeDisposable disposables = new CompositeDisposable();
    private CancellationTokenSource cancellShakeToken = new CancellationTokenSource();
    [SerializeField][Min(0)] private float shakeMultiplier; //This allows to disable or control shake, it's an option in the menu

    private void Awake()
    {
        SignalBus<SignalCameraShake>.Subscribe(x => FireShake(x).Forget()).AddTo(disposables);
    }

    private void OnDestroy()
    {
        disposables.Dispose();
        cancellShakeToken.Cancel();
        cancellShakeToken.Dispose();
    }

    async UniTaskVoid FireShake(SignalCameraShake context)
    {
        //Cancelar token
        GenericExtensions.CancelAndGenerateNew(ref cancellShakeToken);
        await Shake(context.shakeStrengh, context.shakeDuration, cancellShakeToken.Token);
        ResetTransform();
    }

    private void ResetTransform()
    {
        transform.localRotation = Quaternion.identity;
        transform.localPosition = Vector3.zero;
    }

    async UniTask Shake(float shakeStrenght, float shakeDuration, CancellationToken cancellation)
    {
        while (shakeDuration > 0.01f)
        {
            Vector3 rotationAmount = Vector3.forward * Random.Range(0, 1) * shakeStrenght;//A Vector3 to add to the Local Rotation
            Vector2 pos = Random.insideUnitCircle * 0.1f * shakeStrenght * shakeMultiplier + (Vector2)transform.localPosition;
            
            shakeDuration -= Time.unscaledDeltaTime; //Lerp the time, so it is less and tapers off towards the end.

            transform.localRotation = Quaternion.Euler(rotationAmount); //Set the local rotation the be the rotation amount.
            transform.localPosition = pos;

            await UniTask.Yield(PlayerLoopTiming.Update, cancellation);
        }
    }
}