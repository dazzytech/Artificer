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
            * Mathf.Sign(Random.Range(-1f, 1f)));

        float y = position.y + (Random.Range(minDistance, maxDistance)
            * Mathf.Sign(Random.Range(-1f, 1f)));

        return new Vector2(x, y);
    }

    /// <summary>
    /// Returns if a point is within range 
    /// of all four points of a rectangle
    /// </summary>
    /// <param name="point"></param>
    /// <param name="rect"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public static bool InRange(Vector2 point, Rect rect, float range)
    {
        if (rect.Contains(point)) return true;
        if (Vector2.Distance(point, rect.min) < range) return true;
        if (Vector2.Distance(point, rect.min + new Vector2(0, rect.height)) < range) return true;
        if (Vector2.Distance(point, rect.min + new Vector2(rect.width, 0)) < range) return true;
        if (Vector2.Distance(point, rect.max) < range) return true;

        return false;
    }

    /// <summary>
    /// Determines if all points on rect A
    /// are in range of all points on rect B
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public static bool InRange(Rect A, Rect B, float range)
    {
        if (InRange(A.min, B, range))
            return true;
        if (InRange(A.min + new Vector2(0, A.height), B, range))
            return true;
        if (InRange(A.min + new Vector2(A.width, 0), B, range))
            return true;
        if (InRange(A.max, B, range))
            return true;

        return false;
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

    /// <summary>
    /// Returns if a given point in
    /// inside a shape given by an array of vector2
    /// </summary>
    /// <param name="point"></param>
    /// <param name="polygon"></param>
    /// <returns></returns>
    public static bool IsPointInPolygon(Vector2 point, Vector2[] polygon)
    {
        int polygonLength = polygon.Length, i = 0;
        bool inside = false;
        // x, y for tested point.
        float pointX = point.x, pointY = point.y;
        // start / end point for the current polygon segment.
        float startX, startY, endX, endY;
        Vector2 endPoint = polygon[polygonLength - 1];
        endX = endPoint.x;
        endY = endPoint.y;
        while (i < polygonLength)
        {
            startX = endX; startY = endY;
            endPoint = polygon[i++];
            endX = endPoint.x; endY = endPoint.y;
            //
            inside ^= (endY > pointY ^ startY > pointY) /* ? pointY inside [startY;endY] segment ? */
                      && /* if so, test if it is under the segment */
                      ((pointX - endX) < (pointY - endY) * (startX - endX) / (startY - endY));
        }
        return inside;
    }
}

