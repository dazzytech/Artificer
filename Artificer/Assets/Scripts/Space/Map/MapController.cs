using Data.Space;
using Game;
using Space.Segment;
using Space.Ship;
using Stations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Space.Map
{
    /// <summary>
    /// Updates map elements list
    /// in a central area
    /// </summary>
    [RequireComponent(typeof(MapAttributes))]
    public class MapController : NetworkBehaviour
    {
        #region EVENTS

        public delegate void MapUpdate(MapObject obj);

        public static event MapUpdate OnMapUpdate;

        #endregion

        #region ATTRIBUTES

        [SerializeField]
        private MapAttributes m_att;

        #region COLOUR

        [SerializeField]
        private Color m_lowValueAsteroid;

        [SerializeField]
        private Color m_plutAsteroid;

        [SerializeField]
        private Color m_highValueAsteroid;

        [SerializeField]
        private Color m_friendlyColour;

        [SerializeField]
        private Color m_neutralColour;

        [SerializeField]
        private Color m_enemyColour;

        #endregion

        #endregion

        #region ACCESSORS

        public List<MapObject> Map
        {
            get { return m_att.MapItems; }
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Begins the icon population and listens
        /// for item creation
        /// </summary>
        public void InitializeMap()
        {
            // Assign listeners
            SystemManager.Events.EventShipCreated += CreateShip;
            StationController.OnStationCreated += CreateStation;
            SegmentObjectBehaviour.Created += CreateSegmentObject;
            WayPointGenerator.OnWayPointCreated += DeployWaypoint;

            // build objects
            PopulateExistingShips();

            StartCoroutine("UpdateList");
        }

        /// <summary>
        /// Uses linq to find the corresponding map object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public MapObject GetMapObject(Transform obj)
        {
            return m_att.MapItems.
                FirstOrDefault(x => x.Ref == obj);
        }

        #endregion

        #region PRIVATE UTILITIES

        /// <summary>
        /// When the map is initialized
        /// any ships that is already deployed is not added
        /// to the map, do manually here
        /// </summary>
        private void PopulateExistingShips()
        {
            // check that ship container exists
            GameObject shipContainer = GameObject.Find("_ships");
            if (shipContainer == null)
                return;
            
            foreach(Transform ship in shipContainer.transform)
            {
                // all ships here need to be added to the map
                MapObject mapObj = BuildObject
                    (ship);

                mapObj.Type = MapObjectType.SHIP;
                mapObj.TeamID = ship.GetComponent<ShipAccessor>().TeamID;

                if (OnMapUpdate != null)
                    OnMapUpdate(mapObj);
            }
        }

        /// <summary>
        /// Creates a map icon for the
        /// map object
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        private MapObject BuildObject(Transform child)
        {
            MapObject mapObj = m_att.MapItems.FirstOrDefault
                                (x => x.Ref == child);

            if (mapObj == null)
            {
                // object not already added
                mapObj = new MapObject();
                mapObj.Ref = child;
                mapObj.Position = mapObj.Ref.position;
                mapObj.Hidden = false;
                mapObj.TeamID = -1;

                m_att.MapItems.Add(mapObj);
            }

            return mapObj;
        }

        #endregion

        #region EVENT LISTENERS

        /// <summary>
        /// When a ship is added to the screen,
        /// save it to the map objects
        /// </summary>
        /// <param name="CD"></param>
        private void CreateShip(CreateDispatch CD)
        {
            // for now just create the object 
            // (dont need to store additional data this build)
            MapObject mapObj = BuildObject
                (ClientScene.FindLocalObject
                (new NetworkInstanceId(CD.Self)).transform);

            mapObj.Type = MapObjectType.SHIP;
            mapObj.TeamID = CD.TeamID;

            if (OnMapUpdate != null)
                OnMapUpdate(mapObj);
        }

        /// <summary>
        /// When a station is created, add it to
        /// map objects, or in a group if close to another station
        /// </summary>
        /// <param name="station"></param>
        private void CreateStation(StationAccessor station)
        {
            // Initialze our map icon
            MapObject stationMapObject = BuildObject(station.transform);
            stationMapObject.Type = MapObjectType.STATION;
            stationMapObject.TeamID = station.Team.ID;

            // Create a instance of groups for the purpose of not 
            // editing the collection while enumerating through list
            GroupObject group = new GroupObject();

            // Test if this station is within prox of any 
            // other stations of the same team
            foreach(MapObject otherMapObject in m_att.MapItems)
            {
                if (otherMapObject == stationMapObject)
                    continue;

                if (Vector2.Distance(otherMapObject.Position, 
                    stationMapObject.Position) < m_att.GroupProximity &&
                    otherMapObject.TeamID == stationMapObject.TeamID)
                {
                    // these two objects are within range and are in the 
                    // same team, treat differently if station or group object
                    if(otherMapObject.Type == MapObjectType.STATION)
                    {
                        // This is another station
                        // create a new group and place this within
                        group = new GroupObject();
                        group.Type = MapObjectType.TEAM;
                        group.TeamID = station.Team.ID;
                        group.Ref = null;
                        group.Hidden = true;

                        // add other station to the group
                        group.BuildSubObject(otherMapObject);
                    }
                    else if(otherMapObject.Type == MapObjectType.TEAM)
                    {
                        // This is a group that already exists, set this as our group reference
                        group = otherMapObject as GroupObject;
                    }

                    // add this station to the group object
                    group.BuildSubObject(stationMapObject);

                    break;
                }
            }

            // If this object was successfully
            // placed in a group, add the group
            // to map list and nullify the pointer
            if(group != null)
            {
                if (!m_att.MapItems.Contains(group))
                    m_att.MapItems.Add(group);

                // consider updating the group

                group = null;
            }

            // update our object
            if (OnMapUpdate != null)
                OnMapUpdate(stationMapObject);
        }

        /// <summary>
        /// Builds an segment object to the map
        /// </summary>
        /// <param name="segObj"></param>
        /// <param name="trans"></param>
        private void CreateSegmentObject(SegmentObjectData segObj, Transform trans)
        {
            MapObjectType mType = MapObjectType.NULL;
            switch (segObj._type)
            {
                case "_asteroids":
                    mType = MapObjectType.ASTEROID; break;
                case "_debris":
                    mType = MapObjectType.DEBRIS; break;
                case "_satellites":
                    mType = MapObjectType.SATELLITE; break;
            }

            if (mType == MapObjectType.NULL)
                return;

            MapObject mapObj = BuildObject(trans);
            mapObj.Type = mType;
            mapObj.Points = segObj._border;
            mapObj.Size = segObj._size;
            if (mType == MapObjectType.ASTEROID)
            {
                mapObj.Color = segObj._subType == 0? m_lowValueAsteroid: 
                    segObj._subType == 1 ? m_plutAsteroid: m_highValueAsteroid;
            }

            if (OnMapUpdate != null)
                OnMapUpdate(mapObj);
        }

        /// <summary>
        /// invoked when a waypoint is 
        /// placed in space to add to map
        /// </summary>
        /// <param name="position"></param>
        private void DeployWaypoint(Transform waypoint)
        {
            // for now just create the object 
            // (dont need to store additional data this build)
            MapObject mapObj = BuildObject
                (waypoint);

            mapObj.Type = MapObjectType.WAYPOINT;

            mapObj.Color = m_neutralColour;

            if (OnMapUpdate != null)
               OnMapUpdate(mapObj);
        }

        #endregion

        #region COROUTINES

        /// <summary>
        /// Loops through each map
        /// item and update current state
        /// </summary>
        /// <returns></returns>
        private IEnumerator UpdateList()
        {
            while (true)
            {
                for (int i = 0; i < m_att.MapItems.Count; i++)
                {
                    MapObject mObj = m_att.MapItems[i];

                    // update colour based on alignment in client
                    Color newColour = mObj.Relation == -1 ? m_neutralColour :
                        mObj.Relation == 0 ? m_enemyColour : m_friendlyColour;

                    // only update if there is a change
                    bool changed = false;

                    if (mObj.Color != newColour
                        && mObj.TeamID != -1)
                    {
                        mObj.Color = newColour;
                        changed = true;
                    }

                    // If far enough, group far items into their groups
                    if (mObj.Type == MapObjectType.TEAM)
                    {
                        GroupObject gObj = mObj as GroupObject;

                        if (SystemManager.Space.PlayerCamera == null)
                        {
                            if (SystemManager.Space.TeamID == gObj.TeamID)
                            {
                                if (!gObj.Hidden)
                                {
                                    gObj.InRange();
                                    changed = true;
                                }
                            }
                            else
                            {
                                if (gObj.Hidden)
                                {
                                    gObj.OutOfRange();
                                    changed = true;
                                }
                            }
                        }

                        // Check if object is within proximity of player
                        else
                        {
                            float distance = Vector2.Distance(mObj.Position,
                                SystemManager.Space.PlayerCamera.transform.position);

                            if (distance < m_att.ObscureDistance)
                            {
                                if (!gObj.Hidden)
                                {
                                    gObj.InRange();
                                    changed = true;
                                }
                            }
                            else
                            {
                                if (gObj.Hidden)
                                {
                                    gObj.OutOfRange();
                                    changed = true;
                                }
                            }
                        }
                    }
                    // Remove from list if transform is null
                    else if (mObj.Ref == null)
                    {
                        // will dec after completed
                        m_att.MapItems.RemoveAt(i--);
                        changed = true;
                    }

                    else if (mObj.Position.x != mObj.Ref.position.x
                        || mObj.Position.y != mObj.Ref.position.y)
                    {
                        // update location
                        mObj.Position = mObj.Ref.position;
                        changed = true;
                    }
                    else if ((!mObj.Hidden && mObj.Icon != null) ||
                            (mObj.Hidden && mObj.Icon == null))
                        changed = true;

                    // Update any viewers if object is updates
                    if (OnMapUpdate != null && changed)
                        OnMapUpdate(mObj);
                }

                yield return null;
            }
        }

        #endregion
    }
}
