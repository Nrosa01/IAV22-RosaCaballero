using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public static class CustomDebug
{
      public static void DrawLine(Vector3 start, Vector3 end, Color color = default, float width = .02f, float duration = 0.02f)
      {
            GameObject myLine = new GameObject();
            myLine.transform.position = start;
            myLine.AddComponent<LineRenderer>();
            LineRenderer lr = myLine.GetComponent<LineRenderer>();
            lr.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            lr.material.color = color;
            lr.startColor = color;
            lr.endColor = color;
            lr.startWidth = .02f;
            lr.endWidth = .02f;
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
            Object.Destroy(myLine, duration);
      }
      public static void DrawRay(Vector3 start, Vector3 dir, Color color = default, float width = .02f, float duration = 0)
      {
            if (duration == 0)
                  duration = Time.deltaTime;

            DrawLine(start, start + dir, color, width, duration);
      }

      /// <summary> Draw a cross in a world point </summary>
      /// <param name="point">Cross Position</param>
      public static void DrawCross(Vector3 point)
    {
        Color ColorVariation = Color.white / 5;

        Debug.DrawRay(point, Vector3.up * 0.1f, Color.green + ColorVariation);
        Debug.DrawRay(point, Vector3.up * -0.1f, Color.green - ColorVariation);
        Debug.DrawRay(point, Vector3.right * 0.1f, Color.red + ColorVariation);
        Debug.DrawRay(point, Vector3.right * -0.1f, Color.red - ColorVariation);
        Debug.DrawRay(point, Vector3.forward * 0.1f, Color.blue + ColorVariation);
        Debug.DrawRay(point, Vector3.forward * -0.1f, Color.blue - ColorVariation);
    }

      public static void DrawCrossGameMode(Vector3 point)
      {
            Color ColorVariation = Color.white / 5;

            DrawRay(point, Vector3.up * 0.1f, Color.green + ColorVariation);
            DrawRay(point, Vector3.up * -0.1f, Color.green - ColorVariation);
            DrawRay(point, Vector3.right * 0.1f, Color.red + ColorVariation);
            DrawRay(point, Vector3.right * -0.1f, Color.red - ColorVariation);
            DrawRay(point, Vector3.forward * 0.1f, Color.blue + ColorVariation);
            DrawRay(point, Vector3.forward * -0.1f, Color.blue - ColorVariation);
      }

      static float t = 0;
    public static void DrawSphere(Vector3 point, float radius, float duration = 0)
    {
        t += Time.deltaTime * 60;
        t %= 360;

        int max = 24;
        Color ColorVariation = Color.white / 5;
        for (int i = 0; i < max; ++i)
        {
            Debug.DrawRay(point + Quaternion.AngleAxis(t + ((360 / max) * i), Vector3.right) * Vector3.up * radius, Quaternion.AngleAxis(t + ((360 / max) * i), Vector3.right) * Vector3.forward * 0.1f * radius, Color.red + ColorVariation, duration);
            Debug.DrawRay(point + Quaternion.AngleAxis(t + ((360 / max) * i), Vector3.right) * Vector3.up * radius, Quaternion.AngleAxis(t + ((360 / max) * i), Vector3.right) * Vector3.forward * -0.1f * radius, Color.red - ColorVariation, duration);
            Debug.DrawRay(point + Quaternion.AngleAxis(t + ((360 / max) * i), Vector3.forward) * Vector3.up * radius, Quaternion.AngleAxis(t + ((360 / max) * i), Vector3.forward) * Vector3.right * 0.1f * radius, Color.blue + ColorVariation, duration);
            Debug.DrawRay(point + Quaternion.AngleAxis(t + ((360 / max) * i), Vector3.forward) * Vector3.up * radius, Quaternion.AngleAxis(t + ((360 / max) * i), Vector3.forward) * Vector3.right * -0.1f * radius, Color.blue - ColorVariation, duration);
            Debug.DrawRay(point + Quaternion.AngleAxis(t + ((360 / max) * i), Vector3.up) * Vector3.forward * radius, Quaternion.AngleAxis(t + ((360 / max) * i), Vector3.up) * Vector3.right * 0.1f * radius, Color.green + ColorVariation, duration);
            Debug.DrawRay(point + Quaternion.AngleAxis(t + ((360 / max) * i), Vector3.up) * Vector3.forward * radius, Quaternion.AngleAxis(t + ((360 / max) * i), Vector3.up) * Vector3.right * -0.1f * radius, Color.green - ColorVariation, duration);
        }
    }

	public static void DrawSphere(Vector3 point, float radius, Color color, float duration = 0)
	{
		t += Time.deltaTime * 60;
		t %= 360;

		int max = 24;
		for (int i = 0; i < max; ++i)
		{
			Debug.DrawRay(point + Quaternion.AngleAxis(t + ((360 / max) * i), Vector3.right) * Vector3.up * radius, Quaternion.AngleAxis(t + ((360 / max) * i), Vector3.right) * Vector3.forward * 0.1f * radius, color, duration);
			Debug.DrawRay(point + Quaternion.AngleAxis(t + ((360 / max) * i), Vector3.right) * Vector3.up * radius, Quaternion.AngleAxis(t + ((360 / max) * i), Vector3.right) * Vector3.forward * -0.1f * radius, color, duration);
			Debug.DrawRay(point + Quaternion.AngleAxis(t + ((360 / max) * i), Vector3.forward) * Vector3.up * radius, Quaternion.AngleAxis(t + ((360 / max) * i), Vector3.forward) * Vector3.right * 0.1f * radius, color, duration);
			Debug.DrawRay(point + Quaternion.AngleAxis(t + ((360 / max) * i), Vector3.forward) * Vector3.up * radius, Quaternion.AngleAxis(t + ((360 / max) * i), Vector3.forward) * Vector3.right * -0.1f * radius, color, duration);
			Debug.DrawRay(point + Quaternion.AngleAxis(t + ((360 / max) * i), Vector3.up) * Vector3.forward * radius, Quaternion.AngleAxis(t + ((360 / max) * i), Vector3.up) * Vector3.right * 0.1f * radius, color, duration);
			Debug.DrawRay(point + Quaternion.AngleAxis(t + ((360 / max) * i), Vector3.up) * Vector3.forward * radius, Quaternion.AngleAxis(t + ((360 / max) * i), Vector3.up) * Vector3.right * -0.1f * radius, color, duration);
		}
	}



	///<summary> Draws a cross in a world point, with the relative transform axis </summary>
	/// <param name="point">Cross Position </param>
	/// <param name="transform">relative cross directions rotate transform</param>
	public static void DrawCross(Vector3 point, Transform transform)
    {
        Color ColorVariation = Color.white / 5;

        DrawRay(point, transform.up * 0.1f, Color.green + ColorVariation);
        DrawRay(point, transform.up * -0.1f, Color.green - ColorVariation);
        DrawRay(point, transform.right * 0.1f, Color.red + ColorVariation);
        DrawRay(point, transform.right * -0.1f, Color.red - ColorVariation);
        DrawRay(point, transform.forward * 0.1f, Color.blue + ColorVariation);
        DrawRay(point, transform.forward * -0.1f, Color.blue - ColorVariation);
        DrawRay(point, transform.forward * -0.1f, Color.blue - ColorVariation);
    }

	public static void CheckNull(params object[] objects)
	{
		string text = "";
		foreach (object o in objects)
			text += ((o == null ? "null" : "correct") + " ");
		Debug.Log(text);
	}
}
