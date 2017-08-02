using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private Texture2D m_FriendlyStationIcon;

        [SerializeField]
        private Texture2D m_EnemyStationIcon;

        [SerializeField]
        private Texture2D m_RegionIcon;

        #endregion

        #endregion

        #region MONOBEHAVIOUR

        private void OnDisable()
        {
            MapController.OnMapUpdate -= OnIconChanged;
        }

        private void OnDestroy()
        {
            ClearIcons();
        }

        private void OnEnable()
        {
            MapController.OnMapUpdate += OnIconChanged;
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

            // Scale and then position on map
            Vector2 newLoc = mObj.Location;
            newLoc.Scale(m_scale);
            prefab.transform.localPosition = newLoc;
        }

        #endregion

        #region PRIVATE UTILITIES
        
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
                    mObj.Type == MapObjectType.SATELLITE ||
                    mObj.Type == MapObjectType.STATION)
                    // send object to be built
                    BuildIcon(mObj);
                else
                    BuildSegmentIcon(mObj);
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
            if (mObj.Ref != null)
            {
                // create go for our icon
                GameObject GO = new GameObject();
                GO.transform.SetParent(m_baseMap);

                // Create texture
                RawImage img = GO.AddComponent<RawImage>();
                // Set tex based on type
                switch (mObj.Type)
                {
                    case MapObjectType.SHIP:
                        // Resize object based on texture
                        GO.GetComponent<RectTransform>().sizeDelta =
                            new Vector2(30, 30);
                        img.texture = m_shipIcon;
                        break;
                    case MapObjectType.SATELLITE:
                        // Resize object based on texture
                        GO.GetComponent<RectTransform>().sizeDelta =
                            new Vector2(50, 30);
                        img.texture = m_satIcon;
                        break;
                    case MapObjectType.STATION:
                        // Resize object based on texture
                        GO.GetComponent<RectTransform>().sizeDelta =
                            new Vector2(30, 30);
                        if (mObj.TeamID == SystemManager.Space.TeamID)
                            img.texture = m_FriendlyStationIcon;
                        else
                            img.texture = m_EnemyStationIcon;
                        break;
                }

                // Scale and then position on map
                Vector2 newLoc = mObj.Location;
                newLoc.Scale(m_scale);
                GO.transform.localPosition = newLoc;

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
                // create go for our icon
                GameObject Base = new GameObject();
                Base.transform.SetParent(m_baseMap);

                // Create texture
                RawImage img = Base.AddComponent<RawImage>();
                img.texture = m_RegionIcon;

                // Resize object based on texture
                Vector2 newSize = mObj.Size;
                newSize.Scale(m_scale);
                Base.GetComponent<RectTransform>().sizeDelta =
                    newSize;

                // Scale and then position on map
                Vector2 newLoc = mObj.Location;
                newLoc.Scale(m_scale);
                Base.transform.localPosition = newLoc + newSize*.5f;

                mObj.Icon = Base.transform;

                // Create overlaying icon
                GameObject Icon = new GameObject();
                Icon.transform.SetParent(Base.transform);
                Icon.transform.localPosition = new Vector3(0, 0, 0);

                // Create texture
                RawImage iconImg = Icon.AddComponent<RawImage>();
                iconImg.texture = m_RegionIcon;

                switch (mObj.Type)
                {
                    case MapObjectType.ASTEROID:
                        {
                            iconImg.texture = m_astIcon;

                            float scale = Mathf.Max(Mathf.Min(newSize.x, newSize.y), 30);
                            Icon.GetComponent<RectTransform>().sizeDelta =
                                new Vector2(scale, scale);

                            break;
                        }
                    case MapObjectType.DEBRIS:
                        {
                            iconImg.texture = m_wreckIcon;

                            float scale = Mathf.Max(Mathf.Min(newSize.x, newSize.y), 30);
                            Icon.GetComponent<RectTransform>().sizeDelta =
                                new Vector2(scale + scale * .5f, scale);
                            break;
                        }
                }
                
            }
        }

        /// <summary>
        /// Clears all the icons within the map UI
        /// </summary>
        private void ClearIcons()
        {
            foreach(Transform child in m_baseMap.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
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
                if (mObj.Ref == null)
                {
                    // Destroy icon, map controller will delete item
                    GameObject.Destroy(mObj.Icon.gameObject);
                    mObj.Icon = null;
                }
                else
                {
                    // Scale and then reposition on map
                    Vector2 newLoc = mObj.Location;
                    newLoc.Scale(m_scale);
                    mObj.Icon.localPosition = newLoc;
                }
            }
            else
            {
                switch(mObj.Type)
                {
                    case MapObjectType.SHIP:
                    case MapObjectType.SATELLITE:
                    case MapObjectType.STATION:
                        BuildIcon(mObj);
                        break;
                    case MapObjectType.ASTEROID:
                    case MapObjectType.DEBRIS:
                        BuildSegmentIcon(mObj);
                        break;
                }
            }
        }

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

        #endregion
    }
}
