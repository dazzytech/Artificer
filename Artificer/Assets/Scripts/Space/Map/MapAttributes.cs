using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space.Map
{
    // Int enum for the types of objects in map
    // ensure in same order as search items with teams at end
    public enum MapObjectType { SHIP, SATELLITE, ASTEROID, DEBRIS, STATIONA, STATIONB}

    /// <summary>
    /// Storage class
    /// for map items with
    /// info about that item
    /// </summary>
    [System.Serializable]
    public class MapObject
    {
        public Texture2D Icon;
        public Vector2 Location;
        public MapObjectType Type;
        public Transform Ref;
    }

    public class MapAttributes : MonoBehaviour
    {
        [SerializeField]
        public List<MapObject> MapItems;

        // Store a list of strings that Map controller will search through
        // only want objects that have a physical presences
        public string[] SearchItems = 
            { "_ships", "_satellites", "_asteroids", "_debris", "_teams" };
    }
}
