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

        #region MONO BEHAVIOUR

        private void Awake()
        {
            InitializeMap();
        }

        #endregion

        #region PUBLIC INTERACTION

        // Use this for initialization
        public void InitializeMap()
        {
            // Assign listeners
            SystemManager.Events.EventShipCreated += CreateShip;
            StationController.StationCreated += CreateStation;
            SegmentObjectBehaviour.Created += CreateSegmentObject;
            UI.Proxmity.TrackerHUD.OnWayPointCreated += DeployWaypoint;


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

        private MapObject BuildObject(Transform child)
        {
            MapObject mapObj = m_att.MapItems.FirstOrDefault
                                (x => x.Ref == child);

            if (mapObj == null)
            {
                // object not already added
                mapObj = new MapObject();
                mapObj.Ref = child;
                mapObj.Location = mapObj.Ref.position;

                m_att.MapItems.Add(mapObj);
            }

            return mapObj;
        }

        #endregion

        #region EVENT LISTENERS

        private void CreateShip(CreateDispatch CD)
        {
            // for now just create the object 
            // (dont need to store additional data this build)
            MapObject mapObj = BuildObject
                (ClientScene.FindLocalObject
                (new NetworkInstanceId(CD.Self)).transform);

            mapObj.Type = MapObjectType.SHIP;

            if (OnMapUpdate != null)
                OnMapUpdate(mapObj);
        }

        private void CreateStation(StationController station)
        {
            MapObject mapObj = BuildObject(station.transform);
            mapObj.Type = MapObjectType.STATION;
            mapObj.TeamID = station.Att.Team.ID;

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

                    else if (mObj.Location.x != mObj.Ref.position.x
                        || mObj.Location.y != mObj.Ref.position.y)
                        // update location
                        mObj.Location = mObj.Ref.position;
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
