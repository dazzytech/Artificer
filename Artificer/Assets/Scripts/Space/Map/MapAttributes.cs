using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space.Map
{
    // Int enum for the types of objects in map
    // ensure in same order as search items with teams at end
    public enum MapObjectType { NULL, SHIP, SATELLITE, ASTEROID, DEBRIS, STATION, WAYPOINT, TEAM}

    public class MapAttributes : MonoBehaviour
    {
        [SerializeField]
        public List<MapObject> MapItems;

        /// <summary>
        /// Some icons may be grouped into a single icon when this distance away
        /// </summary>
        public float ObscureDistance = 500f;

        /// <summary>
        /// Distance team objects are grouped together
        /// default = 100f
        /// </summary>
        public float GroupProximity = 100f;
    }
}
