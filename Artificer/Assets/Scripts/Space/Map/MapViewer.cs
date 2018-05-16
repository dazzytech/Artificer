using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Space.Map
{
    /// <summary>
    /// Attached to the UI Transfrom 
    /// that displays the map
    /// </summary>
    public class MapViewer : MonoBehaviour, IPointerClickHandler
    {
        #region EVENTS

        public delegate void MapInteract(PointerEventData eventData, Vector2 spaceLocation);

        public event MapInteract OnClick;

        #endregion

        #region ATTRIBUTES

        [Header("HUD Elements")]

        [SerializeField]
        private RectTransform m_baseMap;

        /// <summary>
        /// the direction vector
        /// </summary>
        private Vector2 m_dir;

        private List<MapObject> m_mapObjs
            = new List<MapObject>();

        private Dictionary<MapObject, GameObject> m_prefabs
            = new Dictionary<MapObject, GameObject>();

        #region MAP CUSTOMIZATION

        private MapObjectType[] m_filter;

        private Vector2 m_scale;

        /// <summary>
        /// Used for finding the scaled
        /// mouse position
        /// </summary>
        private Vector2 m_inverseScale;

        private bool m_running = false;

        [SerializeField]
        private bool m_interactive = false;

        #endregion

        #region ICONS

        [Header("Icon Textures")]

        // Create icons for each viewer as may be different

        [SerializeField]
        private Texture2D m_shipIcon;

        [SerializeField]
        private Texture2D m_satIcon;

        [SerializeField]
        private Texture2D m_astIcon;

        [SerializeField]
        private Texture2D m_wreckIcon;

        [SerializeField]
        private Texture2D m_StationIcon;

        [SerializeField]
        private Texture2D m_teamIcon;

        [SerializeField]
        private Texture2D m_waypointIcon;
        
        #endregion

        #endregion

        #region MONOBEHAVIOUR

        private void OnDisable()
        {
            MapController.OnMapUpdate -= OnIconChanged;

            StopCoroutine("DrawBoundaries");
        }

        private void OnDestroy()
        {
            ClearIcons();
        }

        private void OnEnable()
        {
            MapController.OnMapUpdate += OnIconChanged;

            if(m_running)
            {
                ClearIcons();
                BuildIcons();
            }
        }

        public void Start()
        {
            if(!m_running)
                InitializeMap();
        }

        #endregion

        #region PUBLIC INTERACTION

        // Use this for initialization
        public void InitializeMap(MapObjectType[] filter = null)
        {
            // determine our scale
            m_scale.x = Mathf.Round((m_baseMap.rect.width / 5000f) * 100f) / 100f;
            m_scale.y = Mathf.Round((m_baseMap.rect.height / 5000f) * 100f) / 100f;

            m_inverseScale.x = Mathf.Round((5000f/m_baseMap.rect.width) * 100f) / 100f;
            m_inverseScale.y = Mathf.Round((5000f / m_baseMap.rect.height) * 100f) / 100f;

            // apply filter
            m_filter = filter;

            // Start initial map build
            BuildIcons();

            m_running = true;
        }

        /// <summary>
        /// Destroys a given map object and replaces it with
        /// a prefab
        /// </summary>
        /// <param name="mObj"></param>
        /// <param name="prefab"></param>
        public void DeployPrefab(MapObject mObj, GameObject prefab)
        {
            // destroy the texture
            // to replace with our prefab
            if (mObj.Icon != null)
                GameObject.Destroy(mObj.Icon.gameObject);

            mObj.Icon = prefab.transform;

            // put our prefab on map
            prefab.transform.SetParent(m_baseMap);

            // position on map
            prefab.transform.localPosition =
                ScaleAndPosition(mObj.Position);

            if (m_prefabs.ContainsKey(mObj))
            {
                GameObject.Destroy(m_prefabs[mObj]);
                m_prefabs[mObj] = prefab;
            }
            else
                m_prefabs.Add(mObj, prefab);
        }

        public void RotateMap(Vector2 dir)
        {
            if (m_dir != dir)
            {
                m_dir = dir;
                BuildIcons();

                Vector2 offset = new Vector2(2500, 2500);

                foreach (MapObject segmentObject in m_mapObjs)
                {
                    if(segmentObject.Points != null)
                        for (int i = 0; i < 8; i++)
                        {
                            segmentObject.RenderPoints[i] = ApplyCameraRotation
                                (new Vector2(segmentObject.Points[i].x,
                                             segmentObject.Points[i].y), offset);
                        }
                }
            }
        }

        /// <summary>
        /// Moves the base map object to focus on a specific
        /// point in world space
        /// </summary>
        /// <param name="point"></param>
        public void CenterAt(Vector2 point)
        {
            m_baseMap.anchoredPosition = -ScaleAndPosition(point);
        }

        #endregion

        #region PRIVATE UTILITIES

        #region ICONS

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
                (c => m_filter.Contains(c.Type) || m_prefabs.ContainsKey(c)).ToList();

            // create each map object as an icon
            for (int i = 0; i < filteredList.Count; i++)
            {
                MapObject mObj = filteredList[i];

                m_mapObjs.Add(mObj);

                if (mObj.Type == MapObjectType.SHIP ||
                    mObj.Type == MapObjectType.SATELLITE ||
                    mObj.Type == MapObjectType.STATION ||
                    mObj.Type == MapObjectType.TEAM ||
                    mObj.Type == MapObjectType.WAYPOINT)
                    BuildIcon(mObj);
                else
                    BuildSegmentIcon(mObj);

                if (mObj.Hidden)
                    mObj.Icon.GetComponent<RawImage>().enabled = false;
            }
        }

        /// <summary>
        /// Builds an icon that represents 
        /// a single transform in the scene 
        /// with a single texture
        /// </summary>
        /// <param name="mObj"></param>
        private void BuildIcon(MapObject mObj)
        {
            if (mObj.Exists)
            {
                float scale = (m_scale.x < m_scale.y ? m_scale.x : m_scale.y);

                // create go for our icon
                GameObject GO = new GameObject();
                GO.transform.SetParent(m_baseMap);
                if (mObj.Ref != null)
                    GO.name = mObj.Ref.name;
                else
                    GO.name = "group";

                // Create texture
                RawImage img = GO.AddComponent<RawImage>();

                img.color = mObj.Color;

                // Set tex based on type
                switch (mObj.Type)
                {
                    case MapObjectType.SHIP:
                        // Resize object based on texture
                        GO.GetComponent<RectTransform>().sizeDelta =
                            new Vector2(250, 250) * scale;
                        img.texture = m_shipIcon;
                        break;
                    case MapObjectType.SATELLITE:
                        // Resize object based on texture
                        GO.GetComponent<RectTransform>().sizeDelta =
                            new Vector2(250, 125) * scale;
                        img.texture = m_satIcon;
                        break;
                    case MapObjectType.STATION:
                        // Resize object based on texture
                        GO.GetComponent<RectTransform>().sizeDelta =
                            new Vector2(300, 300) * scale;
                            img.texture = m_StationIcon;
                        break;
                    case MapObjectType.TEAM:
                        // Resize object based on texture
                        GO.GetComponent<RectTransform>().sizeDelta =
                            new Vector2(500, 500) * scale;
                        img.texture = m_teamIcon;
                        break;
                    case MapObjectType.WAYPOINT:
                        // Resize object based on texture
                        GO.GetComponent<RectTransform>().sizeDelta =
                            new Vector2(32, 32);
                        img.texture = m_waypointIcon;
                        break;
                }

                GO.transform.localPosition = ScaleAndPosition(mObj.Position);

                mObj.Icon = GO.transform;
            }
        }

        /// <summary>
        /// builds an icon that represents a
        /// field within space e.g. asteroids
        /// </summary>
        /// <param name="mObj"></param>
        private void BuildSegmentIcon(MapObject mObj)
        {
            if (mObj.Ref != null)
            {
                // Resize object based on texture
                Vector2 newSize = mObj.Size;

                newSize.Scale(m_scale);

                float scale = (m_scale.x < m_scale.y ? m_scale.x : m_scale.y);

                if (m_dir.x != 0)
                    newSize = new Vector2(newSize.y, newSize.x);

                if (m_dir.x != 0)
                    newSize *= m_dir.x;
                else if (m_dir.y != 0)
                    newSize *= m_dir.y;

                GameObject region = new GameObject();
                RectTransform rTrans = region.AddComponent<RectTransform>();
                rTrans.SetParent(m_baseMap);
                rTrans.localPosition = ScaleAndPosition(mObj.Position) + (newSize * .5f);


                if (mObj.Points != null)
                {
                    if (mObj.RenderPoints == null)
                    {
                        mObj.RenderPoints = new Vector2[8];
                        mObj.Points.CopyTo(mObj.RenderPoints, 0);
                    }

                    RawImage regionImg = region.AddComponent<RawImage>();
                    regionImg.texture = DrawBoundaries(mObj);
                    regionImg.color = mObj.Color - new Color(0, 0, 0, .5f);

                    rTrans.sizeDelta = newSize;
                }

                // Create overlaying icon
                GameObject icon = new GameObject();
                icon.transform.SetParent(region.transform);
                icon.transform.localPosition = Vector3.zero;

                // Create texture
                RawImage iconImg = icon.AddComponent<RawImage>();

                switch (mObj.Type)
                {
                    case MapObjectType.ASTEROID:
                        {
                            iconImg.texture = m_astIcon;

                            icon.GetComponent<RectTransform>().sizeDelta =
                                new Vector2(250, 250) * scale;

                            iconImg.color = mObj.Color;

                            break;
                        }
                    case MapObjectType.DEBRIS:
                        {
                            iconImg.texture = m_wreckIcon;

                            icon.GetComponent<RectTransform>().sizeDelta =
                                new Vector2(250, 125) * scale;
                            break;
                        }
                }

                mObj.Icon = region.transform;
            }
        }

        /// <summary>
        /// Removes the map icon from the viewer
        /// without affecting the object
        /// </summary>
        /// <param name="mObj"></param>
        private void RemoveIcon(MapObject mObj)
        {
            if(mObj.Icon != null && !m_prefabs.ContainsKey(mObj))
            {
                GameObject.Destroy(mObj.Icon.gameObject);
                mObj.Icon = null;

                if(!mObj.Exists)
                    m_mapObjs.Remove(mObj);
            }
        }

        /// <summary>
        /// Clears all the icons within the map UI
        /// </summary>
        private void ClearIcons()
        {
            foreach(MapObject mObj in m_mapObjs)
            {
                RemoveIcon(mObj);
            }

            m_mapObjs.Clear();
        }

        /// <summary>
        /// Updates an icon when 
        /// the map object is changed in some way
        /// </summary>
        /// <param name="mObj"></param>
        private void OnIconChanged(MapObject mObj)
        {
            if (m_filter != null)
                if (!m_filter.Contains(mObj.Type))
                    return;

            if (mObj.Icon != null)
            {
                if (!mObj.Exists)
                {
                    RemoveIcon(mObj);
                }
                else if(mObj.Hidden)
                {
                    mObj.Icon.GetComponent<RawImage>().enabled = false;
                }
                else
                {
                    mObj.Icon.localPosition = ScaleAndPosition(mObj.Position);
                    RawImage img = mObj.Icon.GetComponent<RawImage>();

                    if (img != null && mObj.Icon.GetComponent<SelectableHUDItem>() == null)
                    {
                        img.color = mObj.Color;
                        img.enabled = true;
                    }
                }
            }
            else
            {
                switch(mObj.Type)
                {
                    case MapObjectType.SHIP:
                    case MapObjectType.SATELLITE:
                    case MapObjectType.STATION:
                    case MapObjectType.TEAM:
                        BuildIcon(mObj);
                        break;
                    case MapObjectType.ASTEROID:
                    case MapObjectType.DEBRIS:
                        BuildSegmentIcon(mObj);
                        break;
                }
            }
        }

        #endregion

        #region CAMERA

        /// <summary>
        /// Positions and scales the map position
        /// based on the map scale, and direction
        /// </summary>
        /// <param name="orig"></param>
        /// <returns></returns>
        private Vector2 ScaleAndPosition(Vector2 orig)
        {
            Vector2 returnVal = orig -new Vector2(2500, 2500);
            returnVal.Scale(m_scale);

            return ApplyCameraRotation(returnVal, Vector2.zero);
        }

        private Vector2 ApplyCameraRotation(Vector2 orig, Vector2 offset = default(Vector2))
        {
            Vector2 returnVal = orig - offset;

            if (m_dir.x != 0)
            {
                returnVal = new Vector2(returnVal.y, -returnVal.x);
                returnVal *= m_dir.x;
            }
            else if (m_dir.y != 0)
                returnVal *= m_dir.y;

            return returnVal + offset;
        }

        #endregion

        /// <summary>
        /// If this is interactive then trigger event
        /// container will be listening to
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (m_interactive)
            {
                if (OnClick != null)
                {
                    Vector2 scaledPoint = Input.mousePosition;
                    Vector3 test = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                    scaledPoint.Scale(m_inverseScale);

                    OnClick(eventData, scaledPoint);
                }
            }
        }

        /// <summary>
        /// Creates a texture that encapsulates the region
        /// </summary>
        /// <param name="mObj"></param>
        /// <returns></returns>
        private Texture2D DrawBoundaries(MapObject mObj)
        {
            Vector2Int sizeInt = new Vector2Int(Mathf.CeilToInt(mObj.Size.x),
                Mathf.CeilToInt(mObj.Size.y));

            // Create a new texture to draw the region to
            Texture2D region = new Texture2D
                (sizeInt.x, sizeInt.y);

            // loop through each pixel and colour white if within boundary
            for (int y = 0; y < sizeInt.x; y++)
            {
                for (int x = 0; x < sizeInt.y; x++)
                {
                    // default solid white so clear any points not in region
                    if (!Math.IsPointInPolygon(new Vector2(x, y) + mObj.Position, mObj.RenderPoints))
                        region.SetPixel(x, y, Color.clear);
                }
            }

            // apply our changes
            region.Apply();

            return region;
        }

        #endregion
    }
}
