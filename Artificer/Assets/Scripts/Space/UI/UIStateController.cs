using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIState { Play, Pause, Popup, TeamPicker, SpawnPicker, Station, Map }

namespace Space.UI
{
    /// <summary>
    /// Manages ui state with the ability to 
    /// set or revert ui state
    /// </summary>
    public class UIStateController : MonoBehaviour
    {
        #region ATTRIBUTES

        [SerializeField]
        private UIChatController m_msg;

        #region HUD ELEMENTS

        [Header("HUD Rects")]

        [SerializeField]
        private GameObject m_playRect;

        [SerializeField]
        private GameObject m_pauseRect;

        [SerializeField]
        private GameObject m_popupRect;

        [SerializeField]
        private GameObject m_teamSelectRect;

        [SerializeField]
        private GameObject m_spawnPickerRect;

        [SerializeField]
        private GameObject m_stationRect;

        [SerializeField]
        private GameObject m_mapRect;

        #endregion

        #region STATE CONTROL

        // State that the UI is currently in
        private UIState m_currentState;

        // State the UI was previously in
        private UIState m_previousState;

        #endregion

        #endregion

        #region ACCESSORS

        public UIState Current
        {
            get
            {
                return m_currentState;
            }
        }

        #endregion

        #region STATE MANAGEMENT

        /// <summary>
        /// Called externally to change the UIState when the game state changes
        /// </summary>
        /// <param name="state"></param>
        public void SetState(UIState state)
        {
            // Cancel if we are already in the state
            if (state == m_currentState)
                return;

            // Add current state to previous
            m_previousState = m_currentState;

            // state new state
            m_currentState = state;

            // update reliant windows
            m_msg.SetState(state);

            ClearRects();

            switch (state)
            {
                case UIState.Pause:
                    m_pauseRect.SetActive(true);
                    break;
                case UIState.Play:
                    m_playRect.SetActive(true);
                    break;
                case UIState.Popup:
                    m_popupRect.SetActive(true);
                    break;
                case UIState.TeamPicker:
                    m_teamSelectRect.SetActive(true);
                    break;
                case UIState.SpawnPicker:
                    m_spawnPickerRect.SetActive(true);
                    break;
                case UIState.Station:
                    m_stationRect.SetActive(true);
                    break;
                case UIState.Map:
                    m_mapRect.SetActive(true);
                    break;
            }
        }

        /// <summary>
        /// Sets the state
        /// to previously assigned state
        /// </summary>
        public void RevertState()
        {
            SetState(m_previousState);
        }

        public void ClearRects()
        {
            m_pauseRect.SetActive(false);
            m_playRect.SetActive(false);
            m_popupRect.SetActive(false);
            m_teamSelectRect.SetActive(false);
            m_spawnPickerRect.SetActive(false);
            m_stationRect.SetActive(false);
            m_mapRect.SetActive(false);
        }

        #endregion
    }
}