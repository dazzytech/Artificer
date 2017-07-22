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

    /// <summary>
    /// Finds the angle difference between the two objects.
    /// </summary>
    /// <returns>The angle difference.</returns>
    /// <param name="trans">Trans.</param>
    /// <param name="dest">Destination.</param>
    public static float Angle(Transform trans, Vector2 point, float homeAngle = 0)
    {
        Vector2 pos = trans.position;
        float angle = Mathf.Atan2(point.y - pos.y, point.x - pos.x) * 180 / Mathf.PI - 90;
        if (homeAngle == 0)
            return Mathf.DeltaAngle(trans.eulerAngles.z, angle);
        else
            return Mathf.DeltaAngle(homeAngle, angle);
    }
}

