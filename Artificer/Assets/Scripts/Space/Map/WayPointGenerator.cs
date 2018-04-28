using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UI;
using Space.UI;
using UnityEngine.EventSystems;

namespace Space.Map
{
    public class WayPointGenerator : MonoBehaviour
    {
        #region EVENTS

        public delegate void ItemCreated(Transform element);

        public static event ItemCreated OnWayPointCreated;

        #endregion

        /// <summary>
        /// Creates waypoints on this container
        /// </summary>
        [SerializeField]
        private Transform m_wayPointContainer;

        [SerializeField]
        private MapViewer m_mapViewer;

        private List<WaypointPrefab> m_waypointList;

        [SerializeField]
        private GameObject m_waypointPrefab;


        #region MONO BEHAVIOUR

        void OnDisable()
        {
            m_mapViewer.OnClick -= OnMapClick;
        }

        void OnEnable()
        {
            m_mapViewer.OnClick += OnMapClick;
        }

        #endregion

        #region PRIVATE UTILITIES

        #region EVENT LISTENER

        /// <summary>
        /// When the player clicks on map
        /// build a transfrom waypoint at that location
        /// and create a map prefab 
        /// </summary>
        /// <param name="eventData"></param>
        private void OnMapClick(PointerEventData eventData,
            Vector2 worldPos)
        {
            // build the empty transform for our 
            // waypoint position
            GameObject wayPoint = new GameObject();

            // add to container
            wayPoint.transform.SetParent
                (m_wayPointContainer);

            wayPoint.transform.position = worldPos;

            OnWayPointCreated(wayPoint.transform);

            // Build the waypoint prefab
            GameObject wayPointObject =
                Instantiate(m_waypointPrefab);

            // Initialize the selectable item
            WaypointPrefab wayPointItem =
                wayPointObject.GetComponent<WaypointPrefab>();
            wayPointItem.Initialize(DeleteWaypoint);
            wayPointItem.Waypoint = wayPoint.transform;

            // Add new prefab to list
            if (m_waypointList == null)
                m_waypointList = new List<WaypointPrefab>();

            m_waypointList.Add(wayPointItem);

            // Update map
            MapObject mObj = SystemManager.Space.GetMapObject
                (wayPoint.transform);

            if (mObj != null)
                m_mapViewer.DeployPrefab(mObj, wayPointObject);
        }

        #endregion

        /// <summary>
        /// Deleted the waypoint we have clicked
        /// </summary>
        /// <param name="ID"></param>
        private void DeleteWaypoint(SelectableHUDItem selected)
        {
            WaypointPrefab waypoint = (WaypointPrefab)selected;

            GameObject.Destroy(waypoint.Waypoint.gameObject);

            waypoint.Waypoint = null;

            m_waypointList.Remove(waypoint);
        }
    }


    #endregion
}