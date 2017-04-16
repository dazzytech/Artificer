using Stations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Space.UI.Station.Map
{
    /// <summary>
    /// Controller object for individual 
    /// Warp Gate HUD Items
    /// </summary>
    public class WarpGatePrefab : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        #region EVENTS

        public delegate void WarpSelected(WarpGatePrefab selected);

        public static event WarpSelected OnWarpSelected;

        #endregion

        #region ATTRIBUTES

        private WarpController m_warpCon;

        private bool m_select;

        #region HUD ELEMENTS

        [Header("HUD Elements")]
        [SerializeField]
        private Text m_name;

        [SerializeField]
        private RawImage m_background;

        #endregion

        #region COLOURS

        [Header("Colours")]

        [SerializeField]
        private Color m_highlight;

        [SerializeField]
        private Color m_selected;

        private Color m_standard;

        #endregion

        #endregion

        #region ACCESSOR

        public WarpController WarpGate
        {
            get { return m_warpCon; }
        }

        #endregion

        #region MONO BEHAVIOUR

        private void Awake()
        {
            m_standard = m_background.color;
        }

        #endregion

        #region PUBLIC INTERACTION

        public void InitializeWarpGate(NetworkInstanceId netID)
        {
            // retreive and store controller
            GameObject GO = 
                ClientScene.FindLocalObject(netID);

            m_warpCon = GO.GetComponent<WarpController>();

            m_name.text = GO.name;
        }

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

        #endregion

        #region POINTER EVENTS

        public void OnPointerClick(PointerEventData eventData)
        {
            // if not already selected process selection
            if (!m_select)
            {
                // trigger an event with out ship info
                OnWarpSelected(this);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!m_select)
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
