using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Space.Map
{
    /// <summary>
    /// Attached to the UI Transfrom 
    /// that displays the map
    /// </summary>
    public class MapViewer : MonoBehaviour
    {
        #region ATTRIBUTES

        [Header("HUD Elements")]

        [SerializeField]
        private RectTransform m_baseMap;

        #region MAP CUSTOMIZATION

        private MapObjectType[] m_filter;

        private Vector2 m_scale;

        private bool m_running = false;

        #endregion

        // Create icons for each viewer as may be different

        #region ICONS

        [Header("Icon Textures")]

        [SerializeField]
        private Texture2D m_shipIcon;

        [SerializeField]
        private Texture2D m_satIcon;

        [SerializeField]
        private Texture2D m_astIcon;

        [SerializeField]
        private Texture2D m_wreckIcon;

        [SerializeField]
        private Texture2D m_AIcon;

        [SerializeField]
        private Texture2D m_BIcon;

        #endregion

        #endregion

        #region MONOBEHAVIOUR

        private void OnDisable()
        {
            MapController.OnMapUpdate -= MapChanged;

            StopCoroutine("BuildIcons");

            ClearIcons();
        }

        private void OnEnable()
        {
            if (m_running)
            {
                MapController.OnMapUpdate += MapChanged;
                StartCoroutine("BuildIcons");
            }
        }

        #endregion

        #region PUBLIC INTERACTION

        // Use this for initialization
        public void InitializeMap(MapObjectType[] filter)
        {
            // determine our scale
            m_scale.x = m_baseMap.rect.width / 5000f;
            m_scale.y = m_baseMap.rect.height / 5000f;

            // apply filter
            m_filter = filter;

            // Add listeners
            MapController.OnMapUpdate += MapChanged;

            // Start initial map build
            StartCoroutine("BuildIcons");

            m_running = true;
        }

        #endregion

        #region PRIVATE UTILITIES

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

        private void BuildIcon(MapObject mObj)
        {
            if(mObj.Ref != null)
            {
                // create go for our icon
                GameObject GO = new GameObject();
                GO.transform.SetParent(m_baseMap);

                // Create texture
                RawImage img = GO.AddComponent<RawImage>();
                // Set tex based on type
                switch(mObj.Type)
                {
                    case MapObjectType.SHIP:
                        img.texture = m_shipIcon;
                        break;
                    case MapObjectType.SATELLITE:
                        img.texture = m_satIcon;
                        break;
                    case MapObjectType.ASTEROID:
                        img.texture = m_astIcon;
                        break;
                    case MapObjectType.DEBRIS:
                        img.texture = m_wreckIcon;
                        break;
                    case MapObjectType.STATIONA:
                        img.texture = m_AIcon;
                        break;
                    case MapObjectType.STATIONB:
                        img.texture = m_BIcon;
                        break;
                }

                // Scale and then position on map
                Vector2 newLoc = mObj.Location;
                newLoc.Scale(m_scale);
                GO.transform.localPosition = newLoc;

                // Resize object based on texture
                GO.GetComponent<RectTransform>().sizeDelta =
                    new Vector2(30, 30);

                mObj.Icon = GO.transform;
            }
        }

        private void MapChanged(MapObject mObj)
        {
            if (m_filter.Contains(mObj.Type))
            {
                if (mObj.Icon != null)
                    GameObject.Destroy(mObj.Icon.gameObject);

                if (mObj.Ref != null)
                    BuildIcon(mObj);
                else
                    mObj.Icon = null;
            }
        }

        #endregion

        #region COROUTINE

        /// <summary>
        /// Creates an icon for each item
        /// </summary>
        /// <returns></returns>
        private IEnumerator BuildIcons()
        {
            ClearIcons();

            // Retrieve list based on what filters we are having
            List<MapObject> filteredList = SystemManager.Space.Map.Where
                (c => m_filter.Contains(c.Type)).ToList();

            // create each map object as an icon
            for (int i = 0; i < filteredList.Count; i++)
            {
                MapObject mObj = filteredList[i];

                // filter will be applied here

                // send object to be built
                BuildIcon(mObj);

                yield return null;
            }

            yield return null;
        }

        #endregion
    }
}
