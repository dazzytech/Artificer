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
    public class ShipSelectItem : MonoBehaviour, 
        IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        #region EVENTS

        public delegate void ShipSelected(ShipSelectItem selected);

        public static event ShipSelected OnShipSelected;

        #endregion

        #region ATTRIBUTES

        [SerializeField]
        private ShipData m_shipData;

        private bool m_select;

        #region HUD ELEMENTS

        [Header("HUD Elements")]
        [SerializeField]
        private Text m_header;

        [SerializeField]
        private Text m_desc;

        [SerializeField]
        private Image m_background;

        #endregion

        #region COLOURS

        [Header("Colours")]

        [SerializeField]
        private Color m_highlight;

        [SerializeField]
        private Color m_selected;

        private Color m_standard;

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

        private void Awake()
        {
            m_standard = m_background.color;
        }

        #endregion

        #region PUBLIC INTERACTION

        public void Deselect()
        {
            m_select = false;
            m_background.color = m_standard;
        }

        public void Select()
        {
            m_select = true;
            m_background.color = m_selected;
        }

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

            // keep reference
            m_shipData = newShip;

            // build ship to window
            m_builder.BuildShipData(m_shipData, m_piecePrefab);

            m_select = false;
        }

        #endregion

        #region POINTER EVENTS

        public void OnPointerClick(PointerEventData eventData)
        {
            // if not already selected process selection
            if (!m_select)
            {
                // trigger an event with out ship info
                OnShipSelected(this);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(!m_select)
            {
                m_background.color = m_highlight;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!m_select)
            {
                m_background.color = m_standard;
            }
        }

        #endregion
    }
}