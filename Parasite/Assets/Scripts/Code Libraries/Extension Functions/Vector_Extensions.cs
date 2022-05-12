using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public static class Vector_Extensions
{
    // Returns Vector2.zero if boolean is false
    public static Vector2 ZeroIfFalse(this Vector2 vectot2, bool boolean) => boolean ? vectot2 : Vector2.zero;

    public static Vector2 GetInverted(this Vector2 vector2) => new Vector2(vector2.y, vector2.x);

    public static Vector2 Rotated(this Vector2 v, float delta)
    {
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        );
    }

    public static void Rotate(this Vector2 v, float delta)
    {
        v = new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        );
    }

    public static float SquaredDistance(Vector2 a, Vector2 b) => (a - b).sqrMagnitude;
    public static float DistanceFast(Vector2 a, Vector2 b) => MathsFast.FastMagnitude(a - b);

    public static Vector3 Rounded(this Vector3 v) => new Vector3(Mathf.Round(v.x), Mathf.Round(v.y), Mathf.Round(v.z));

    public static float Angle2D(Vector2 from, Vector2 to)
    {
        Vector2 direction = (to - from).normalized;
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }

    public static Vector3 Slerp(Vector3 start, Vector3 end, Vector3 center, float t)
    {
        var startRelativeCenter = start - center;
        var endRelativeCenter = end - center;

        return Vector3.Slerp(startRelativeCenter, endRelativeCenter, t) + center;
    }


    public static Vector3 SetX(this Vector3 vec, float x) => new Vector3(x, vec.y, vec.z);
    public static Vector3 SetY(this Vector3 vec, float y) => new Vector3(vec.x, y, vec.z);
    public static Vector3 SetZ(this Vector3 vec, float z) => new Vector3(vec.x, vec.y, z);
}

public static class MathsFast
{
    public static float FastMagnitude(Vector2 vector)
    {
        return FastInverseSqrt((vector.x * vector.x) + (vector.y * vector.y));
    }

    public static float FastInverseSqrt(float z)
    {
        if (z == 0) return 0;
        FloatIntUnion u;
        u.tmp = 0;
        u.f = z;
        u.tmp -= 1 << 23; /* Subtract 2^m. */
        u.tmp >>= 1; /* Divide by 2. */
        u.tmp += 1 << 29; /* Add ((b + 1) / 2) * 2^m. */
        return u.f;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct FloatIntUnion
    {
        [FieldOffset(0)]
        public float f;

        [FieldOffset(0)]
        public int tmp;
    }
}