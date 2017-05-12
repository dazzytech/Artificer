using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UI;
using System;
using Data.Shared;

namespace Space.UI.Spawn
{
    public class ShipSelectItem : SelectableHUDItem
    {
        #region ATTRIBUTES

        [Header("Ship Information")]

        [SerializeField]
        private ShipData m_shipData;

        #region HUD ELEMENTS

        [Header("HUD Elements")]
        [SerializeField]
        private Text m_header;

        [SerializeField]
        private Text m_type;

        [SerializeField]
        private Text m_desc;

        #endregion

        #region SHIP BUILDER

        [Header("Ship Builder")]

        [SerializeField]
        private ComponentBuilderUtility m_builder;

        [SerializeField]
        private GameObject m_piecePrefab;

        #endregion

        #endregion

        #region ACCESSOR

        public string Name
        {
            get { return m_shipData.Name; }
        }

        #endregion

        #region MONO BEHAVIOUR

        private void OnDestroy()
        {
            m_builder.ClearShip();
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// When UI item is created
        /// assign ship data to item and
        /// display properties
        /// </summary>
        /// <param name="newShip"></param>
        public void AssignShipData(ShipData newShip)
        {
            // Display Title and description
            m_header.text = newShip.Name;
            m_desc.text = newShip.Description;
            m_type.text = newShip.Category;

            // keep reference
            m_shipData = newShip;

            // build ship to window
            m_builder.BuildShipData(m_shipData, m_piecePrefab);
        }

        #endregion
    }
}