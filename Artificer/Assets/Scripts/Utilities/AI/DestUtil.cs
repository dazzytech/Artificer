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
            if(!hit.transform.IsChildOf(trans))
            {
                return hit.collider.transform;
            }
        }
        return null;
    }
}
