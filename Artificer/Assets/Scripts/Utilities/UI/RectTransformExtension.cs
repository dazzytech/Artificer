using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Effects
{
    public static class RectTransformExtension
    {
        public static bool InBounds(RectTransform r, Vector3 pos)
        {
            Vector3[] worldCorners = new Vector3[4];
            r.GetWorldCorners(worldCorners);

            if (pos.x >= worldCorners[0].x && pos.x < worldCorners[2].x
                && pos.y >= worldCorners[0].y && pos.y < worldCorners[2].y)
            {
                return true;
            }

            return false;
        }
    }
}