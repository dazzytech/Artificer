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

        #endregion

        #region ACCESSORS

        public List<MapObject> Map
        {
            get { return m_att.MapItems; }
        }

        #endregion

        #region PUBLIC INTERACTION

        // Use this for initialization
        public void InitializeMap()
        {
            // Assign listeners
            SystemManager.Events.EventShipCreated += CreateShip;
            StationController.OnStationCreated += CreateStation;
            SegmentObjectBehaviour.Created += CreateSegmentObject;
            UI.Proxmity.TrackerHUD.OnWayPointCreated += DeployWaypoint;

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
            foreach(Transform ship in 
                GameObject.Find("_ships").transform)
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
            MapObject mapObj = BuildObject(station.transform);
            mapObj.Type = MapObjectType.STATION;
            mapObj.TeamID = station.Team.ID;

            // Test if this station is within prox of any 
            // other stations of the same team
            foreach(MapObject mObj in m_att.MapItems)
            {
                if(Vector2.Distance(mObj.Position, 
                    mapObj.Position) < m_att.GroupProximity &&
                    mObj.TeamID == mapObj.TeamID)
                {
                    // This is the group that the team will be stored within 
                    GroupObject group = null;

                    // these two objects are within range and are in the 
                    // same team, treat differently if station or group object
                    if(mObj.Type == MapObjectType.STATION)
                    {
                        // This is another station
                        // create a new group and place this within
                        group = new GroupObject();
                        group.Type = MapObjectType.TEAM;
                        group.TeamID = station.Team.ID;
                        group.Ref = null;

                        // add other station to the group
                        group.BuildSubObject(mObj);

                        // remove other station from map objs
                        m_att.MapItems.Remove(mObj);
                    }
                    else if(mObj.Type == MapObjectType.TEAM)
                    {
                        // This is a group that already exists, set this as our group reference
                        group = mObj as GroupObject;
                    }

                    // add this station to the group object
                    group.BuildSubObject(mapObj);

                    // remove from group
                    m_att.MapItems.Remove(mapObj);
                }
            }

            if (OnMapUpdate != null)
                OnMapUpdate(mapObj);
        }

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

                    // Remove from list if transform is null
                    // e.g. object destroyed
                    if (mObj.Ref == null)
                        m_att.MapItems.RemoveAt(i--); // will dec after completed

                    if (mObj is GroupObject)
                    {
                        GroupObject gObj = mObj as GroupObject;

                        if(SystemManager.Space.PlayerCamera != null)
                        {
                            if(!gObj.Hidden)
                            { }
                        }
                        // Check if object is within proximity of player
                        //if (Vector2.Distance(mObj.Position, ))
                    }

                    else if (mObj.Position.x != mObj.Ref.position.x
                        || mObj.Position.y != mObj.Ref.position.y)
                    {
                        // update location
                        mObj.Position = mObj.Ref.position;
                    }
                    else
                        continue;

                    // Update any viewers
                    if (OnMapUpdate != null)
                        OnMapUpdate(mObj);
                }

                yield return null;
            }
        }

        #endregion
    }
}
