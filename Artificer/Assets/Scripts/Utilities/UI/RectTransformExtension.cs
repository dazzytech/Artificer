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

        /// <summary>
        /// Returns a new position based on the boundries 
        /// of the provided parent rect
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parent"></param>
        /// <param name="newPosition"></param>
        /// <returns></returns>
        public static Vector2 RestrictBounds(RectTransform child, RectTransform parent, Vector2 newPosition)
        {
            float parentLeft =
            (parent.position.x -
            (parent.GetComponent<RectTransform>().rect.width * .5f)
            + (child.rect.width * .5f));

                float parentRight =
                    (parent.position.x +
                    (parent.GetComponent<RectTransform>().rect.width * .5f))
                    - (child.rect.width * .5f);

                float parentBottom = (parent.position.y -
                    (parent.GetComponent<RectTransform>().rect.height * .5f))
                    + (child.rect.height * .5f);

                float parentTop = (parent.position.y +
                    (parent.GetComponent<RectTransform>().rect.height * .5f))
                    - (child.rect.height * .5f);

            newPosition.x = Mathf.Clamp(newPosition.x, parentLeft, parentRight);
            newPosition.y = Mathf.Clamp(newPosition.y, parentBottom, parentTop);

            return newPosition;
        }
    }
}