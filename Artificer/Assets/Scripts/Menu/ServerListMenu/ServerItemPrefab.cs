using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

//Artificer
using Data.UI;

namespace Menu.Server
{
    /// <summary>
    /// Controller script for server list item
    /// displays information regarding server
    /// and keeps a reference to the server
    /// </summary>
    public class ServerItemPrefab : MonoBehaviour, 
        IPointerClickHandler, 
        IPointerEnterHandler,
        IPointerExitHandler
    {
        #region EVENTS

        public delegate void ClickedEvent(ServerItemPrefab serverData);

        public static event ClickedEvent OnClickedEvent;

        #endregion

        #region ATTRIBUTES

        // Reference to the server information
        private ServerData m_serverData;

        private bool m_selected;

        #region HUD ELEMENTS

        [Header("HUD Elements")]

        [SerializeField]
        private Text m_serverName;

        [SerializeField]
        private Text m_gameType;

        [SerializeField]
        private Text m_serverVersion;

        [SerializeField]
        private Text m_playerCount;

        [SerializeField]
        private Image m_selfPanel;

        #endregion

        #region COLOURS

        [Header("Colours")]

        private Color m_standardColour;

        [SerializeField]
        private Color m_highlightColour;

        [SerializeField]
        private Color m_selectedColour;

        #endregion

        #region ACCESSORS

        public ServerData Server
        {
            get { return m_serverData; }
        }

        #endregion

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Creates reference to the data and displays 
        /// info 
        /// </summary>
        /// <param name="serverData"></param>
        public void BuildServer(ServerData serverData)
        {
            // Retrieve this colour
            m_standardColour = m_selfPanel.color;

            m_selected = false;

            // Reference
            m_serverData = serverData;

            // Display information
            m_serverName.text = m_serverData.ServerName;

            m_gameType.text = "default";

            m_serverVersion.text = m_serverData.ServerVersion;

            /*m_playerCount.text = string.Format("{0}/{1}",
                m_serverData.ServerPopulation, *
                m_serverData.ServerMaxPopulation);*/
        }

        /// <summary>
        /// Listener sets item as selected or deselected
        /// </summary>
        /// <param name="selected"></param>
        public void SetSelected(bool selected)
        {
            m_selected = selected;

            if (selected)
                m_selfPanel.color = m_selectedColour;
            else
                m_selfPanel.color = m_standardColour;
        }

        #endregion

        #region IPOINTEREVENTS

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(!m_selected)
                m_selfPanel.color = m_highlightColour;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!m_selected)
                m_selfPanel.color = m_standardColour;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClickedEvent(this);
        }

        #endregion
    }
}