using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space.Map
{
    /// <summary>
    /// A map object that stores a list of 
    /// sub map objects
    /// </summary>
    [System.Serializable]
    public class GroupObject : MapObject
    {
        /// <summary>
        /// Hides this group when not hidden
        /// </summary>
        [SerializeField]
        public List<MapObject> SubObjects;

        /// <summary>
        /// If true then display sub nodes, else display team node
        /// </summary>
        public bool ViewRange;

        /// <summary>
        /// Adds object to child list and 
        /// reposition to the center
        /// </summary>
        /// <param name="mObj"></param>
        public void BuildSubObject(MapObject mObj)
        {
            // add the obj to the sub array
            if (SubObjects == null)
                SubObjects = new List<MapObject>();
            SubObjects.Add(mObj);

            // Find the center of all the components by 
            // finding the average of the x and y seperately
            float totalX = 0, totalY = 0;

            // accumulate totals
            foreach (MapObject sObj in SubObjects)
            {
                totalX += sObj.Position.x;
                totalY += sObj.Position.y;
            }

            // assign average to location so icon is in center
            Position.x = totalX / SubObjects.Count;
            Position.y = totalY / SubObjects.Count;
        }

        /// <summary>
        /// Sets this team object to hide and then
        /// unhides all sub nodes
        /// </summary>
        public void InRange()
        {
            Hidden = true;

            foreach (MapObject sObj in SubObjects)
                sObj.Hidden = false;
        }

        /// <summary>
        /// Sets this team object to show and then
        /// hides all sub nodes
        /// </summary>
        public void OutOfRange()
        {
            Hidden = false;

            foreach (MapObject sObj in SubObjects)
                sObj.Hidden = true;
        }
    }
}
