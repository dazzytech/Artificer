using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space.Map
{
    // Int enum for the types of objects in map
    // ensure in same order as search items with teams at end
    public enum MapObjectType { NULL, SHIP, SATELLITE, ASTEROID, DEBRIS, STATION, WAYPOINT, TEAM}

    /// <summary>
    /// Storage class
    /// for map items with
    /// info about that item
    /// </summary>
    [System.Serializable]
    public class MapObject
    {
        /// <summary>
        /// GUI icon that shows
        /// </summary>
        public Transform Icon;
        /// <summary>
        /// position where the icon is displayed
        /// </summary>
        public Vector2 Position;
        /// <summary>
        /// For areas, width and height of the icon
        /// </summary>
        public Vector2 Size;
        /// <summary>
        /// Original points that define the borders of a segment
        /// </summary>
        public Vector2[] Points;
        /// <summary>
        /// Points that are manipulated within the map controller
        /// and used by GL to draw bounds
        /// </summary>
        public Vector2[] RenderPoints;
        /// <summary>
        /// What type of object we are displaying
        /// </summary>
        public MapObjectType Type;
        /// <summary>
        /// Reference to the object to check for updates
        /// </summary>
        public Transform Ref;
        /// <summary>
        /// What team, if any, this object belongs to
        /// </summary>
        public int TeamID;
        /// <summary>
        /// If true, this object is not displayed
        /// </summary>
        public bool Hidden;
    }

    /// <summary>
    /// A map object that stores a list of 
    /// sub map objects
    /// </summary>
    [System.Serializable]
    public class GroupObject:MapObject
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
            foreach(MapObject sObj in SubObjects)
            {
                totalX += sObj.Position.x;
                totalY += sObj.Position.y;
            }

            // assign average to location so icon is in center
            Position.x = totalX / SubObjects.Count;
            Position.y = totalY / SubObjects.Count;
        }

    }

    public class MapAttributes : MonoBehaviour
    {
        [SerializeField]
        public List<MapObject> MapItems;

        /// <summary>
        /// Some icons may be grouped into a single icon when this distance away
        /// </summary>
        public float ObscureDistance;

        /// <summary>
        /// Distance team objects are grouped together
        /// </summary>
        public float GroupProximity;
    }
}
