using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space.Map
{
    // Int enum for the types of objects in map
    // ensure in same order as search items with teams at end
    public enum MapObjectType { NULL, SHIP, SATELLITE, ASTEROID, DEBRIS, STATION, WAYPOINT}

    /// <summary>
    /// Storage class
    /// for map items with
    /// info about that item
    /// </summary>
    [System.Serializable]
    public class MapObject
    {
        public Transform Icon;
        public Vector2 Location;
        public Vector2 Size;
        public Vector2[] Points;
        public MapObjectType Type;
        public Transform Ref;
        public int TeamID;
    }

    public class MapAttributes : MonoBehaviour
    {
        [SerializeField]
        public List<MapObject> MapItems;
    }
}
