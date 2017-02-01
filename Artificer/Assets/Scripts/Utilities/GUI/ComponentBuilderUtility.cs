using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Space.Ship;
using Space.Ship.Components.Listener;

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
        private Transform m_selfPanel;

        // created transform the components will be placed on
        private Transform m_constructPanel;

        // Prefab object - component Icon
        [Header("Build Item Prefab")]
        [SerializeField]
        private GameObject m_componentPrefab;

        //dimensions of transform
        private float m_width, m_height;

        // refence to ship attributes
        private ShipAttributes m_ship;

        #endregion

        #region MONO BEHAVIOUR

        // Use this for initialization
        void Awake()
        {
            // If not assigned beforehand,
            // Set to this panel
            if (m_selfPanel == null)
                m_selfPanel = this.transform;
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Builds ship gameobject to define UI panel
        /// </summary>
        /// <param name="Ship"></param>
        public void BuildShip(ShipAttributes Ship)
        {
            m_ship = Ship;

            // Begin routines
            StartCoroutine("DiscoverSize");
        }

        #endregion

        #region COROUTINE

        /// <summary>
        /// Loop through each component
        /// and resize total size for component
        /// </summary>
        /// <returns></returns>
        private IEnumerator DiscoverSize()
        {
            // Create for floats for min and max sizes for ship transform
            float minX, minY, maxX, maxY;
            minX = minY = maxX = maxY = 0;

            // Find min and max points of total ship 
            // using each component
            foreach (ComponentListener item in m_ship.Components)
            {
                // if any of the points are out of bounds from
                // component min and max then replace
                if (minX > item.Min.x) minX = item.Min.x;
                if (minY > item.Min.y) minY = item.Min.y;
                if (maxX > item.Max.x) maxX = item.Max.x;
                if (maxY > item.Max.y) maxY = item.Max.y;

                yield return null;
            }

            // Now we have our dimentions
            // create new transform
            m_width = maxX - minX;
            m_height = maxY - minY;

            StartCoroutine("BuildComponents");

            yield break;
        }

        /// <summary>
        /// Place components on construct panel
        /// </summary>
        /// <returns></returns>
        private IEnumerator BuildComponents()
        {
            GameObject constructGO = new GameObject("ConstructPanel");
            m_constructPanel = constructGO.transform;
            m_constructPanel.localScale = new Vector3(m_width, m_height, 0);

            yield break;
        }

        #endregion
    }
}
