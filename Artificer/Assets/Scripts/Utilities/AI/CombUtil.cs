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

    public static Transform EnemyIsVisible
        (Vector3 dest, float dist, Transform trans, Transform target)
    {
        Vector3 pos = trans.position;
        Vector3 dir = (dest - pos).normalized;
        
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

