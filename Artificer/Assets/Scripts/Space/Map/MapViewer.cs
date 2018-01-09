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

        /// <summary>
        /// the direction vector
        /// </summary>
        private Vector2 m_dir;

        private List<MapObject> m_mapObjs
            = new List<MapObject>();

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

        [SerializeField]
        private Material m_regionMaterial;

        #endregion

        #region COLOURS

        [Header("Colours")]

        [SerializeField]
        private Color m_defaultAsteroidColour;

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

            StartCoroutine("DrawBoundaries");

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
                    for (int i = 0; i < 8; i++)
                    {
                        segmentObject.RenderPoints[i] = ApplyCameraRotation
                            (new Vector2(segmentObject.Points[i].x,
                                         segmentObject.Points[i].y), offset);
                    }
                }
            }
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
                if (mObj.Points != null)
                {
                    if (!m_mapObjs.Contains(mObj))
                    {
                        if (mObj.RenderPoints == null)
                        {
                            mObj.RenderPoints = new Vector2[8];
                            mObj.Points.CopyTo(mObj.RenderPoints, 0);
                        }
                        m_mapObjs.Add(mObj);
                    }
                }
                else
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

                    if (m_dir.x != 0)
                        newSize = new Vector2(newSize.y, newSize.x);

                    Base.GetComponent<RectTransform>().sizeDelta =
                        newSize;

                    if (m_dir.x != 0)
                        newSize *= m_dir.x;
                    else if (m_dir.y != 0)
                        newSize *= m_dir.y;

                    Base.transform.localPosition = ScaleAndPosition(mObj.Position) + (newSize * .5f);

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

                                float scale = Mathf.Max(
                                    Mathf.Min(newSize.x, newSize.y), 30);

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
                if (mObj.Ref == null)
                {
                    // Destroy icon, map controller will delete item
                    GameObject.Destroy(mObj.Icon.gameObject);
                    mObj.Icon = null;
                    m_mapObjs.Remove(mObj);
                }
                else
                {
                    mObj.Icon.localPosition = ScaleAndPosition(mObj.Position);
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

        #endregion

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

        #region GL

        private IEnumerator DrawBoundaries()
        {
            if (m_regionMaterial == null)
                yield break;

            while (true)
            {
                // We should only read the screen buffer after rendering is complete
                yield return new WaitForEndOfFrame();

                GL.PushMatrix();
                m_regionMaterial.SetPass(0);
                GL.LoadPixelMatrix(SystemManager.Size.x, SystemManager.Size.width, SystemManager.Size.y, SystemManager.Size.height);
                GL.Begin(GL.QUADS);
                GL.Color(m_defaultAsteroidColour);

                foreach (MapObject segmentObject in m_mapObjs)
                {
                    // Temp fix, only works for regions with 
                    // 8 points
                    GL.Vertex3(segmentObject.RenderPoints[0].x, segmentObject.RenderPoints[0].y, 0);
                    GL.Vertex3(segmentObject.RenderPoints[1].x, segmentObject.RenderPoints[1].y, 0);
                    GL.Vertex3(segmentObject.RenderPoints[2].x, segmentObject.RenderPoints[2].y, 0);
                    GL.Vertex3(segmentObject.RenderPoints[3].x, segmentObject.RenderPoints[3].y, 0);

                    GL.Vertex3(segmentObject.RenderPoints[3].x, segmentObject.RenderPoints[3].y, 0);
                    GL.Vertex3(segmentObject.RenderPoints[4].x, segmentObject.RenderPoints[4].y, 0);
                    GL.Vertex3(segmentObject.RenderPoints[7].x, segmentObject.RenderPoints[7].y, 0);
                    GL.Vertex3(segmentObject.RenderPoints[0].x, segmentObject.RenderPoints[0].y, 0);

                    GL.Vertex3(segmentObject.RenderPoints[4].x, segmentObject.RenderPoints[4].y, 0);
                    GL.Vertex3(segmentObject.RenderPoints[5].x, segmentObject.RenderPoints[5].y, 0);
                    GL.Vertex3(segmentObject.RenderPoints[6].x, segmentObject.RenderPoints[6].y, 0);
                    GL.Vertex3(segmentObject.RenderPoints[7].x, segmentObject.RenderPoints[7].y, 0);
                }

                GL.End();
                GL.PopMatrix();
            }
        }

        #endregion
    }
}
