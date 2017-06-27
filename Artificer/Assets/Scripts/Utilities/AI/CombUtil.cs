using UnityEngine;
using System.Collections;

public class CombUtil
{
    public static bool BehindEnemy(Transform trans, Transform enemy)
    {
        // Enemy is in front?
        float toEnemyAngle = DestUtil.FindAngleDifference(trans, enemy.position);

        if (toEnemyAngle < 20 && toEnemyAngle > -20)
        {
            // Enemy is in front of us
            float enemyFacing = DestUtil.FindAngleDifference(enemy, trans.position);
            if(enemyFacing < 160 && enemyFacing > -160)
            {
                // We are tailing enemy
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Returns the transform of any collider
    /// between this ship and its target
    /// </summary>
    /// <param name="dest"></param>
    /// <param name="dist"></param>
    /// <param name="trans"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static Transform ObjectIsVisible
        ( Transform trans, Transform target)
    {
        if (target == null)
            return null;

        Vector3 pos = trans.position;
        float dist = Vector3.Distance(target.position, pos);
        Vector3 dir = (target.position - pos).normalized;
        
        // raycast of short distance to detect if an object is between self and enemy
        // push origin forward so shooters body isnt affected
        RaycastHit2D ray = Physics2D.Raycast
            (pos + (dir*3f), dir, dist);
        
        if (ray.transform != null && !ray.transform.Equals(target)
            && !ray.transform.Equals(trans))
            return ray.transform;
        else 
            return null;
    }
}

