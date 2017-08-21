using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

// Arificer
using Data.Space;
using Space.Ship;
using UI.Effects;
using Space.Ship.Components.Listener;
using Space.Map;
using UnityEngine.EventSystems;
using UI;

namespace Space.UI.Proxmity
{
    #region MARKER CONTAINER

    /// <summary>
    /// Contaier class for tracking information 
    /// regarding transforms
    /// </summary>
    public class Marker: IndexedObject
    {
        public MapObject trackedObj;
        public GameObject arrow;
        public GameObject box;
        public GameObject text;
        public Vector3 direction;
        public float trackDist;
    }

    #endregion

    /// <summary>
    /// Tracks nearby transforms using tags
    /// to identify and displays in circle
    /// </summary>
    public class TrackerHUD : SelectableHUDList
    {
        #region EVENTS

        public delegate void ItemCreated(Transform element);

        public static event ItemCreated OnWayPointCreated;

        #endregion

        #region ATTRIBUTES

        [Header("Tracker HUD")]

        #region MARKER LISTS

        private IndexedList<Marker> m_markers;

        private Marker[] m_compass = new Marker[4];

        #endregion

        #region HUD ELEMENTS

        [Header("Prefabs")]

        [SerializeField]
        private GameObject m_arrowPrefab;

        [SerializeField]
        private GameObject m_compassPrefab;

        [SerializeField]
        private GameObject m_boxPrefab;

        [SerializeField]
        private GameObject m_waypointPrefab;

        [Header("HUD Element")]

        [SerializeField]
        private ProximityHUD m_proximity;

        /// <summary>
        /// Creates waypoints on this container
        /// </summary>
        [SerializeField]
        private Transform m_wayPointContainer;

        [SerializeField]
        private MapViewer m_mapViewer;

        #endregion

        #region COLOUR

        [Header("Colour")]

        [SerializeField]
        private Color m_standardColor;

        [SerializeField]
        private Color m_friendlyColour;

        [SerializeField]
        private Color m_enemyColour;

        [SerializeField]
        private Color m_friendlyStation;

        [SerializeField]
        private Color m_enemyStation;

        [SerializeField]
        private Color m_compassColour;

        #endregion

        #region MISC ATTRIBUTES

        // Font used to display distance text
        [Header("Font")]

        [SerializeField]
        private Font m_font;

        [Header("Customizable")]

        [SerializeField]
        // radius of the UI circle
        private float m_radius = 100f;

        private MapObjectType[] m_filter;

        private bool m_running = false;

        private GameObject m_cameraObject;

        private List<WaypointPrefab> m_waypointList;

        private Vector2 m_dir;

        #region COMPASS

        private Vector2[] m_compassDirs =
            new Vector2[4] {new Vector2(0,1) ,
                new Vector2(1, 0), new Vector2(0, -1), new Vector2(-1, 0) };

        private string[] m_compassNames
            = new string[4] { "Up", "Right", "Down", "Left" }; 

        private float m_compassRadius = 200f;

        #endregion

        #endregion

        #endregion

        #region MONO BEHAVIOUR

        protected override void OnDisable()
        {
            if (m_running)
            {
                MapController.OnMapUpdate -= OnIconChanged;

                m_mapViewer.OnClick -= OnMapClick;
            }

            if(SystemManager.Space != null)
                SystemManager.Space.OnOrientationChange -= OnOrientationChange;
        }

        protected override void OnEnable()
        {
            if (m_running)
            {
                MapController.OnMapUpdate += OnIconChanged;

                m_mapViewer.OnClick += OnMapClick;
            }

            SystemManager.Space.OnOrientationChange += OnOrientationChange;
        }

        public void Start()
        {
            if (!m_running)
                InitializeTracker();

            m_cameraObject = GameObject.FindGameObjectWithTag
                    ("MainCamera");

            // Build the compass
            BuildCompassMarkers();
        }

        private void OnDestroy()
        {
            ClearIcons();
        }

        void LateUpdate()
        {
            // If we don't have a camera or anything to track
            // this stops here
            if (m_cameraObject == null || m_markers == null)
                return;

            foreach(Marker m in m_compass)
            {
                RepositionCompass(m);
            }

            foreach (Marker m in m_markers)
            {
                if (m.arrow == null || m.box == null
                   || m.text == null)
                    BuildMarker(m, m_cameraObject.transform.position);
                else
                    RepositionMarker(m, m_cameraObject.transform.position, m.ID);
            }
        }

        #endregion

        #region PUBLIC INTERACTION

        public override void SelectItem(int ID, bool multiSelect)
        {
            if (!multiSelect)
                ClearAll();

            SelectableHUDItem[] items = m_prefabList.Where
                (x => x.SharedIndex == ID).ToArray();

            foreach (SelectableHUDItem item in items)
                item.Select();
        }

        public override void HoverItem(int ID, bool multiSelect)
        {
            if (!multiSelect)
                ClearAll();

            SelectableHUDItem[] items = m_prefabList.Where
                (x => x.SharedIndex == ID).ToArray();

            foreach(SelectableHUDItem item in items)
                item.Highlight(true);
        }

        public override void LeaveItem(int ID)
        {
            SelectableHUDItem[] items = m_prefabList.Where
                (x => x.SharedIndex == ID).ToArray();

            foreach (SelectableHUDItem item in items)
                item.Highlight(false);
        }

        #endregion

        #region PRIVATE UTILITIES

        /// <summary>
        /// Quickly builds the tracking hud using 
        /// the map
        /// </summary>
        private void InitializeTracker()
        {
            m_markers = new IndexedList<Marker>();

            // Add listeners
            MapController.OnMapUpdate += OnIconChanged;

            // Start initial map build
            BuildIcons();

            m_running = true;
        }

        /// <summary>
        /// Creates an icon for each item
        /// </summary>
        /// <returns></returns>
        private void BuildIcons()
        {
            ClearIcons();

            List<MapObject> filteredList;

            if (m_filter == null)
                filteredList = SystemManager.Space.Map;
            else
                // Retrieve list based on what filters we are having
                filteredList = SystemManager.Space.Map.Where
                (c => m_filter.Contains(c.Type)).ToList();

            // create each map object as an icon
            for (int i = 0; i < filteredList.Count; i++)
            {
                MapObject mObj = filteredList[i];

                if (mObj.Type == MapObjectType.SHIP ||
                    mObj.Type == MapObjectType.STATION)
                    // send object to be built
                    AddMarker(mObj);
            }
        }

        /// <summary>
        /// Clears all the icons within the map UI
        /// </summary>
        private void ClearIcons()
        {
            foreach (Transform child in m_body)
            {
                GameObject.Destroy(child.gameObject);
            }

            if(m_markers != null)
                m_markers.Clear();
        }

        /// <summary>
        /// Updates an icon when 
        /// object is changes
        /// not including location
        /// </summary>
        /// <param name="mObj"></param>
        private void OnIconChanged(MapObject mObj)
        {
            if (m_filter != null)
                if (!m_filter.Contains(mObj.Type))
                    return;

            Marker m = m_markers.
                        FirstOrDefault(x =>
                        x.trackedObj == mObj);
            if (m != null)
            {
                if (mObj.Ref == null)
                {
                    if(m != null)
                        DeleteMarker(m);
                }
            }
            else
            if (mObj.Ref != null)
            {
                if (mObj.Type == MapObjectType.SHIP 
                    || mObj.Type == MapObjectType.STATION 
                    || mObj.Type == MapObjectType.WAYPOINT)
                    // send object to be built
                    AddMarker(mObj);
            }
        }

        /// <summary>
        /// If the angles of the two
        /// objects are about to cause the
        /// text to overlap: push one forward
        /// </summary>
        /// <param name="index"></param>
        /// <param name="m"></param>
        /// <param name="cameraPosition"></param>
        /// <returns></returns>
        private int Overlap(int index, Marker m, Vector2 cameraPosition)
        {
            int overlap = 40;

            Vector2 dirA =
                (m.trackedObj.Location - cameraPosition);

            for (; index > 0;)
            {
                Marker testM = m_markers[--index];
                if (testM.trackedObj == null)
                    continue;
                else if (!testM.arrow.activeSelf)
                    continue;

                Vector2 dirB =
                    (testM.trackedObj.Location - cameraPosition);

                float angle = Vector2.Angle(dirA, dirB);
                if (angle < 10f)
                {
                    overlap += 40;
                }
            }

            return overlap;
        }

        #region MARKER

        /// <summary>
        /// Called externally when theres an object we want to track
        /// typically by the spawner
        /// </summary>
        /// <param name="piece"></param>
        public void AddMarker(MapObject piece)
        {
            if (piece.Ref.tag == "PlayerShip")
                return;

            // stations aren't added this way so piece is a ship
            Marker m = m_markers.FirstOrDefault(x => x.trackedObj == piece);

            if (m == null)
            { 
                m = new Marker();
                m.trackedObj = piece;
                m_markers.Add(m);
            }
        }

        /// <summary>
        /// Clears away all hud elements of the marker
        /// and deletes
        /// </summary>
        /// <param name="m"></param>
        private void DeleteMarker(Marker m)
        {
            GameObject.Destroy(m.arrow);
            GameObject.Destroy(m.box);
            GameObject.Destroy(m.text);

            m_markers.Remove(m);
        }

        /// <summary>
        /// Constructs the marker based on the objects tag
        /// and places in the correct position around the ship
        /// </summary>
        /// <param name="m"></param>
        /// <param name="camPos"></param>
        private void BuildMarker(Marker m, Vector2 camPos)
        {
            // face the marker pointing towards the object
            Vector2 dir = (m.trackedObj.Location
                    - camPos).normalized * m_radius;

            // Generic text object within marker to display
            // the distance to the object 
            m.text = new GameObject();

            // Add text component
            Text mtext = m.text.AddComponent<Text>();

            // Place the text as a child of this HUD
            m.text.transform.SetParent(m_body, false);

            // Initialize text format 
            mtext.font = m_font;
            mtext.alignment = TextAnchor.MiddleCenter;
            mtext.fontSize = 8;

            // Initialize the HUD items
            m.arrow = (GameObject)Instantiate(m_arrowPrefab, dir, Quaternion.identity);

            SelectableHUDItem arrow = m.arrow.GetComponent<SelectableHUDItem>();
            arrow.Initialize(m_proximity.Select, m_proximity.Hover, m_proximity.Leave);
            m.arrow.transform.SetParent(m_HUD.transform, false);

            
            m.box = (GameObject)Instantiate(m_boxPrefab, dir, Quaternion.identity);
            SelectableHUDItem box = m.box.GetComponent<SelectableHUDItem>();
            box.Initialize(m_proximity.Select, m_proximity.Hover, m_proximity.Leave);
            m.box.transform.SetParent(m_HUD.transform, false);

            m.box.SetActive(false);
            m.arrow.SetActive(false);

            // assign UI elements based on tag (for now)
            switch (m.trackedObj.Type)
            {
                case MapObjectType.SHIP:
                    {
                        Color usedColour = Color.white;

                        if (m.trackedObj.TeamID == SystemManager.Space.TeamID)
                        {
                            usedColour = m_friendlyColour;
                        }
                        else
                        {
                            usedColour = m_enemyColour;
                        }

                        int ID = (int)m.trackedObj.Ref.GetComponent
                                <ShipAccessor>().NetID.Value;
                        
                        arrow.SetColour(usedColour);
                        arrow.SharedIndex = ID;
                        
                        box.SetColour(usedColour);
                        box.SharedIndex = ID;

                        m_prefabList.Add(arrow);
                        m_prefabList.Add(box);

                        mtext.color = usedColour;

                        m.trackDist = 500f;
                        break;
                    }
                case MapObjectType.STATION:
                    {
                        Color usedColour = Color.white;

                        if (m.trackedObj.TeamID == SystemManager.Space.TeamID)
                        {
                            usedColour = m_friendlyStation;
                        }
                        else
                        {
                            usedColour = m_enemyStation;
                        }

                        int ID = (int)m.trackedObj.Ref.GetComponent
                                <Stations.StationController>().netId.Value;
                        
                        arrow.SetColour(usedColour);
                        arrow.SharedIndex = ID;

                        box.SetColour(usedColour);
                        box.SharedIndex = ID;
                        m_prefabList.Add(arrow);
                        m_prefabList.Add(box);

                        mtext.color = usedColour;

                        m.box.transform.localScale = new Vector3(2, 2, 1);
                        m.trackDist = 500f;
                        break;
                    }
                default:
                    // Grey if a different tag              
                    mtext.color = m_standardColor;

                    arrow.SetColour(m_standardColor);

                    box.SetColour(m_standardColor);

                    m.trackDist = -1f;
                    break;
            }
        }

        /// <summary>
        /// Sets the piece arrow and box visiblity.
        /// depending on position and distance of tracked
        /// object.
        /// </summary>
        /// <returns>The piece distance to the object.</returns>
        /// <param name="m">M.</param>
        private float SetMarkerVisiblity(Marker m, Vector3 camPos)
        {
            // distance between our ship and tracked object
            float objDistance = Vector3.Distance(m.trackedObj.Location, camPos);
            // Retreive borders of the screen game is being played on
            Bounds camB = CameraExtensions.OrthographicBounds(Camera.main);

            if (camB.Contains(m.trackedObj.Location))
            {
                if (!m.box.activeSelf)
                {
                    // object is visible to use
                    // only show box directly over obj
                    m.arrow.SetActive(false);
                    m.box.SetActive(true);
                    m.text.SetActive(false);

                    //PanelFadeEffects.FlashInItem(m.box.GetComponent<Image>());
                }
            }
            else
            {
                // Object is offscreen
                // Display arrow if within tracking range
                if (m.trackDist == -1 || objDistance <= m.trackDist)
                {
                    if (!m.arrow.activeSelf)
                    {
                        m.arrow.SetActive(true);
                        m.text.SetActive(true);

                        //PanelFadeEffects.FlashInItem(m.arrow.GetComponent<Image>());
                    }
                }
                else
                {
                    m.arrow.SetActive(false);
                    m.text.SetActive(false);
                }
                m.box.SetActive(false);
            }

            return objDistance;
        }

        /// <summary>
        /// respostion GUI Elements based on tracked object position
        /// </summary>
        /// <param name="m"></param>
        /// <param name="camPos"></param>
        /// <param name="i"></param>
        private void RepositionMarker(Marker m, Vector2 camPos, int i)
        {
            if (m.trackedObj.Ref == null)
                return;

            float objDistance = SetMarkerVisiblity(m, camPos);

            Vector2 dir =
                (m.trackedObj.Location - camPos)
                    .normalized * m_radius;

            if (m_dir.x != 0)
            {
                dir = new Vector2(dir.y, -dir.x);
                dir *= m_dir.x;
            }
            else if (m_dir.y != 0)
                dir *= m_dir.y;

            m.arrow.transform.up = dir.normalized;
            m.arrow.transform.localPosition = dir;

            RectTransform boxRect = m.box.GetComponent<RectTransform>();
            //now you can set the position of the ui element
            boxRect.anchoredPosition =
                UIConvert.WorldToCamera(m.trackedObj.Ref);

            m.text.transform.localPosition = dir + dir.normalized * Overlap(i, m, camPos);
            m.text.GetComponent<Text>().text = ((int)objDistance * 0.01).ToString("F2") + "km";
        }

        #region COMPASS

        private void RepositionCompass(Marker m)
        {
            Vector2 dir = m.direction * m_compassRadius;

            if (m_dir.x != 0)
            {
                dir = new Vector2(dir.y, -dir.x);
                dir *= m_dir.x;
            }
            else if (m_dir.y != 0)
                dir *= m_dir.y;

            m.arrow.transform.up = dir.normalized;
            m.arrow.transform.localPosition = dir;

            m.text.transform.localPosition = dir + (dir.normalized * 20);
        }

        private void BuildCompassMarkers()
        {
            for (int i = 0; i < m_compassDirs.Length; i++)
            {
                m_compass[i] = new Marker();

                m_compass[i].direction = m_compassDirs[i];

                m_compass[i].arrow = (GameObject)Instantiate(m_compassPrefab, 
                    m_compassDirs[i], Quaternion.identity);

                m_compass[i].arrow.transform.SetParent(m_HUD.transform, false);

                // Generic text object within marker to display
                // the distance to the object 
                m_compass[i].text = new GameObject();

                // Add text component
                Text mtext = m_compass[i].text.AddComponent<Text>();

                // Place the text as a child of this HUD
                m_compass[i].text.transform.SetParent(m_body, false);

                // Initialize text format 
                mtext.font = m_font;
                mtext.alignment = TextAnchor.MiddleCenter;
                mtext.fontSize = 8;

                mtext.text = m_compassNames[i];

                // Grey if a different tag              
                mtext.color = m_standardColor;

                m_compass[i].arrow.GetComponent<SelectableHUDItem>().SetColour(m_compassColour);

                m_compass[i].arrow.GetComponent<SelectableHUDItem>().FlashImage();
            }

            Invoke("FadeCompass", 5.0f);
        }

        private void FadeCompass()
        {
            for (int i = 0; i < 4; i++)
            {
                m_compass[i].arrow.GetComponent<SelectableHUDItem>().FadeImage();
                m_compass[i].text.gameObject.SetActive(false);
            }
        }

        #endregion

        #endregion

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

        /// <summary>
        /// Called when the player changes the 
        /// camera view, rebuild and rotate the viewer
        /// </summary>
        /// <param name="newOrient"></param>
        private void OnOrientationChange(Vector2 newOrient)
        {
            m_mapViewer.RotateMap(newOrient);

            m_dir = newOrient;

            for(int i = 0; i < 4; i++)
            {
                m_compass[i].arrow.GetComponent<SelectableHUDItem>().FlashImage();
                m_compass[i].text.gameObject.SetActive(true);
            }

            Invoke("FadeCompass", 5.0f);
        }

        #endregion

        #endregion
    }
}
