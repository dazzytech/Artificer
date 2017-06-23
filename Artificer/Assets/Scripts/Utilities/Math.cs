using UnityEngine;
using System.Collections;

public class Math
{
    public static float Round(float value, int digits)
    {
        float mult = Mathf.Pow(10.0f, (float)digits);
        return Mathf.Round(value * mult) / mult;
    }

    public static Vector3 RotateAroundPoint(Vector3 point, Vector3 pivot, Quaternion angle)
    {
        return angle * (point - pivot) + pivot;
    }

    public static Vector2 RandomWithinRange(Vector3 position, float minDistance, float maxDistance)
    {
        // create an x value between maxDistance to -maxDistance 
        float x = position.x + (Random.Range(minDistance, maxDistance) 
            * Mathf.Abs(Random.Range(-1f, 1f)));

        float y = position.y + (Random.Range(minDistance, maxDistance)
            * Mathf.Abs(Random.Range(-1f, 1f)));

        return new Vector2(x, y);
    }
}

