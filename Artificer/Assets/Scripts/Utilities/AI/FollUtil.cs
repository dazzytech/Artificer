using UnityEngine;
using System.Collections;

public class FollUtil 
{
    /// <summary>
    /// Finds the local position of the ship within a circle.
    /// </summary>
    /// <returns>The local position circle.</returns>
    /// <param name="radius">Radius.</param>
    /// <param name="distance">Distance.</param>
    public static Vector2 FindLocalPositionCircle
        (float radius, float distance)
    {
        float posX = (Random.Range(radius, distance)) * Mathf.Sign(Random.Range(-1, 1));
        float posY = (Random.Range(radius, distance)) * Mathf.Sign(Random.Range(-1, 1));
        Vector2 position = new Vector2
            (posX,posY);

        // This would be where I test agaisnt other ships
        return position;
    }

    public static Vector2 FindWorldPositionCircle
        (Vector2 local, Transform follow)
    {
        Vector2 position = new Vector2
            (follow.position.x + local.x,
             follow.position.y + local.y);
        
        // This would be where I test agaisnt other ships
        return position;
    }
}

