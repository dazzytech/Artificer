using UnityEngine;
using System.Collections;
using UI;
using Space.Ship;

namespace Space.UI.Ship
{
    /// <summary>
    /// Container object for ship highlight 
    /// object for Target HUD
    /// </summary>
    public class TargetShipItem : MonoBehaviour
    {
        #region ATTRIBUTES

        [Header("Reference")]

        [SerializeField]
        private Transform m_self;

        public ComponentBuilderUtility Builder;

        [HideInInspector]
        public ShipAttributes Ship;

        [Header("Prefabs")]

        [SerializeField]
        private GameObject m_UIComponentPrefab;

        [Header("Colours")]

        [SerializeField]
        private Color m_prefabColour;

        private bool m_active = false;

        #endregion

        #region MONOBEHAVIOUR

        void Update()
        {
            if(m_active)
            {
                // As this is worldUI
                // place directly over ship object
                m_self.position = Ship.transform.position;

                m_self.localRotation = Ship.transform.localRotation;
            }
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Initialised the ship target item
        /// - builds ship components HUD
        /// - Triggers tracking function
        /// </summary>
        /// <param name="ship"></param>
        public void BuildShip(ShipAttributes ship, Color prefabColour)
        {
            Ship = ship;

            m_prefabColour = prefabColour;

            Builder.BuildShip(Ship, m_UIComponentPrefab);

            // recolour here
            foreach (ViewerItem item
                in Builder.ViewerItems)
                item.SetColour(m_prefabColour);

            m_active = true;
        }

        /// <summary>
        /// Clears UI components
        /// </summary>
        public void ClearShip()
        {
            Builder.ClearShip();

            m_active = false;
        }

        #endregion
    }
}
