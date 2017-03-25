using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Space.Ship;
using Space.Ship.Components.Listener;
using System.Collections.Generic;
using Data.Shared;

namespace UI
{
    /// <summary>
    /// Placed onto ship trackers
    /// that will build ship components given to it
    /// </summary>
    public class ComponentBuilderUtility : MonoBehaviour
    {
        #region ATTRIBUTES

        // Self reference to the transform we will build 
        // on
        [Header("Component Build Transform")]
        [SerializeField]
        private RectTransform m_selfPanel;

        // created transform the components will be placed on
        private RectTransform m_constructPanel;

        // Prefab object - component Icon
        private GameObject m_componentPrefab;

        //dimensions of transform
        private float m_width, m_height, m_margin;

        // refence to ship attributes
        private ShipAttributes m_shipAtt;

        // Keep track of component list
        private List<ViewerItem> m_viewerItems;

        #endregion

        #region ACCESSORS

        public List<ViewerItem> ViewerItems
        {
            get { return m_viewerItems; }
        }

        #endregion

        #region MONO BEHAVIOUR

        // Use this for initialization
        void Awake()
        {
            // If not assigned beforehand,
            // Set to this panel
            if (m_selfPanel == null)
                m_selfPanel = GetComponent<RectTransform>();
        }

        #endregion

        #region PUBLIC INTERACTION
        
        /// <summary>
        /// Takes the ship data as a param
        /// builds the ship temporarily and 
        /// sends that to the ship builder
        /// then clears shipGO
        /// </summary>
        /// <param name="newShip">New ship.</param>
        public void BuildShipData
            (ShipData ship, GameObject PiecePrefab)
        {
            GameObject tempShip = new GameObject();

            ShipAttributes tempAtt = 
                tempShip.AddComponent<ShipAttributes>();

            tempShip.AddComponent<ShipMessageController>();

            tempShip.AddComponent<Rigidbody2D>();

            Space.Segment.Generator.ShipGenerator.
                GenerateShip(ship, tempShip);

            BuildShip(tempAtt, PiecePrefab);

            Destroy(tempShip);
        }

        /// <summary>
        /// Builds ship gameobject to define UI panel
        /// </summary>
        /// <param name="Ship"></param>
        public void BuildShip
            (ShipAttributes Ship, GameObject PiecePrefab)
        {
            m_shipAtt = Ship;

            m_componentPrefab = PiecePrefab;

            m_viewerItems = new List<ViewerItem>();

            // clear prev ship if exists
            if (m_constructPanel != null)
                Destroy(m_constructPanel.gameObject);

            // Begin routines
            if(m_shipAtt != null)
                DiscoverSize();
        }

        public void ClearShip()
        {
            m_shipAtt = null;

            m_componentPrefab = null;

            if (m_constructPanel != null)
                Destroy(m_constructPanel.gameObject);
        }

        #endregion

        #region PRIVATE UTILITES

        /// <summary>
        /// Build the construction rect transform
        /// that the ship is built on and scales 
        /// it to our dimensions
        /// </summary>
        private void BuildConstructGO()
        {
            GameObject constructGO = new GameObject("ConstructPanel");

            m_constructPanel = constructGO.AddComponent<RectTransform>();
            // Set parent
            m_constructPanel.SetParent(m_selfPanel, false);
            // Set size
            m_constructPanel.sizeDelta = new Vector2(m_width * 100, m_height * 100);

            // if self panel size is 0 then game size
            if (m_selfPanel.rect.width == 0 
                && m_selfPanel.rect.height == 0)
            {
                // resize item to fit in world space
                m_constructPanel.transform.localScale = new Vector3(.01f, .01f, 1);
                m_margin *= 2.5f;
                return;
            }

            // if self panel is wider than taller the ship is required to be lengthways
            if (m_selfPanel.sizeDelta.x > m_selfPanel.sizeDelta.y)
            {
                m_constructPanel.localEulerAngles = new Vector3(0, 0, 270);

                // Return percentage scale of panels based on width
                float heightScale = m_selfPanel.rect.size.y / m_constructPanel.rect.size.y;
                m_constructPanel.transform.localScale = new Vector3(heightScale, heightScale, 1);
            }
            else
            {
                // Return percentage scale of panels based on width
                float widthScale = m_selfPanel.rect.size.x / m_constructPanel.rect.size.x;
                m_constructPanel.transform.localScale = new Vector3(widthScale, widthScale, 1);
            }
        }

        /// <summary>
        /// Loop through each component
        /// and resize total size for component
        /// </summary>
        /// <returns></returns>
        private void DiscoverSize()
        {
            // Create for floats for min and max sizes for ship transform
            float minX, minY, maxX, maxY;
            minX = minY = maxX = maxY = 0;

            if (m_shipAtt == null)
                return;

            // Find min and max points of total ship 
            // using each component
            for (int i = 0; i < m_shipAtt.Components.Count; i++)
            {
                if (m_shipAtt == null)
                    return;

                ComponentListener item = m_shipAtt.Components[i];

                if (item == null)
                {
                    continue;
                }

                // if any of the points are out of bounds from
                // component min and max then replace
                if (minX > item.Min.x) minX = item.Min.x;
                if (minY > item.Min.y) minY = item.Min.y;
                if (maxX < item.Max.x) maxX = item.Max.x;
                if (maxY < item.Max.y) maxY = item.Max.y;
            }

            // Now we have our dimentions
            // create new transform
            m_width = maxX - minX;
            m_height = maxY - minY;

            m_margin = maxY;

            BuildConstructGO();

            BuildComponents();

            return;
        }

        /// <summary>
        /// Place components on construct panel
        /// </summary>
        /// <returns></returns>
        private void BuildComponents()
        {
            if(m_shipAtt == null)
                return;

            // Find min and max points of total ship 
            // using each component
            for (int i = 0; i < m_shipAtt.Components.Count; i++)
            {
                if (m_shipAtt == null)
                    return;

                ComponentListener component = m_shipAtt.Components[i];

                if (component == null)
                {
                    continue;
                }

                // Create GameObject
                GameObject newObj = (GameObject)Instantiate(m_componentPrefab);
                newObj.layer = this.gameObject.layer;
                // Set anchor for accurate positioning
                newObj.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1.0f);
                newObj.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1.0f);
                newObj.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                // Position component offsettinng height
                newObj.transform.position = (component.Postion * 100) - new Vector3(0, m_margin * 100, 0);
                // Set parent
                newObj.transform.SetParent(m_constructPanel, false);

                // Create Viewer Item
                ViewerItem item = newObj.GetComponent<ViewerItem>();
                item.Define(component.gameObject, component.ID);
                m_viewerItems.Add(item);
            }
            return;
        }

        #endregion
    }
}