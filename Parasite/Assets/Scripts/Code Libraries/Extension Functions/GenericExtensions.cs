using Random = UnityEngine.Random;
using System.Threading;

public static class GenericExtensions
{
    public static T GetRandom<T>(this T[] array) => array[Random.Range(0, array.Length)];

    public static void CancelAndGenerateNew(ref CancellationTokenSource source)
    {
        source?.Cancel();
        source = new CancellationTokenSource();
    }
}
