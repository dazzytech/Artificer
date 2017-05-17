using Space.Segment;
using Space.Ship;
using Stations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Space.Map
{
    /// <summary>
    /// Updates map elements list
    /// in a central area
    /// </summary>
    [RequireComponent(typeof(MapAttributes))]
    public class MapController : MonoBehaviour
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

        private void Awake()
        {
            InitializeMap();
        }

        #region PUBLIC INTERACTION

        // Use this for initialization
        public void InitializeMap()
        {
            // Assign listeners
            ShipInitializer.OnShipCreated += CreateShip;
            StationController.StationCreated += CreateStation;
            SegmentObjectBehaviour.Created += CreateSegmentObject;

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
                mapObj.Location = child.position;
                mapObj.Ref = child;

                m_att.MapItems.Add(mapObj);
            }

            return mapObj;
        }

        #endregion

        #region EVENT LISTENERS

        private void CreateShip(ShipAttributes ship)
        {
            // for now just create the object 
            // (dont need to store additional data this build)
            MapObject mapObj = BuildObject(ship.transform);
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

            mapObj.Size = segObj._size;

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
                for(int i = 0; i < m_att.MapItems.Count; i++)
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
