using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DestUtil 
{
    public static Transform FrontIsClear
        (Transform trans, float distance, float arc)
    {
        RaycastHit2D[] hits =
            Physics2D.CircleCastAll(trans.position, distance, Vector2.zero);

        foreach (RaycastHit2D hit in hits)
        {
            if(hit.transform != trans)
            {
                Vector2 angleToObj = 
                    (hit.point - new Vector2(trans.position.x,
                      trans.position.y) ).normalized;
                if(Vector2.Angle(trans.up, angleToObj) < arc)
                    return hit.collider.transform;
            }
        }

        return null;
    }

    /// <summary>
    /// Finds the angle difference between the two objects.
    /// </summary>
    /// <returns>The angle difference.</returns>
    /// <param name="trans">Trans.</param>
    /// <param name="dest">Destination.</param>
    public static float FindAngleDifference(Transform trans, Vector3 dest)
    {
        Vector2 pos = trans.position;
        float angle = Mathf.Atan2(dest.y-pos.y, dest.x-pos.x)*180 / Mathf.PI -90;
        return Mathf.DeltaAngle(trans.eulerAngles.z, angle);
    }

    /// <summary>
    /// Check whether the next random position is the same as current position
    /// </summary>
    /// <param name="pos">position to check</param>
    public static bool IsInCurrentRange
        (Transform trans, Vector3 pos,
         float minDistance, float maxDistance)
    {
        float xPos = Mathf.Abs(pos.x - trans.position.x);
        float yPos = Mathf.Abs(pos.y - trans.position.y);
        
        if (xPos >= minDistance && yPos >= minDistance &&
            xPos <= maxDistance && yPos <= maxDistance)
            return true;
        
        return false;
    }

    /// <summary>
    /// Used by AI systems to avoid colloisions 
    /// with objects
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    public static Transform ObjectWithinProximity(Transform trans, float radius)
    {
        RaycastHit2D[] hits =
            Physics2D.CircleCastAll(trans.position, radius, Vector2.zero);
        
        foreach (RaycastHit2D hit in hits)
        {
            if(hit.transform != trans)
            {
                return hit.collider.transform;
            }
        }
        return null;
    }
}
