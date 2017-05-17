using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space.UI
{
    public enum UIState { Play, Pause, Popup, TeamPicker, SpawnPicker, Station, Map }

    /// <summary>
    /// Manages ui state with the ability to 
    /// set or revert ui state
    /// </summary>
    public class UIStateController : MonoBehaviour
    {
        #region ATTRIBUTES

        [SerializeField]
        private UIMessageController m_msg;

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

        #endregion

        #region STATE CONTROL

        // State that the UI is currently in
        private UIState m_currentState;

        // State the UI was previously in
        private UIState m_previousState;

        #endregion

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

            switch (state)
            {
                case UIState.Pause:
                    m_pauseRect.SetActive(true);
                    m_playRect.SetActive(false);
                    m_popupRect.SetActive(false);
                    m_teamSelectRect.SetActive(false);
                    m_spawnPickerRect.SetActive(false);
                    m_stationRect.SetActive(false);
                    break;
                case UIState.Play:
                    m_pauseRect.SetActive(false);
                    m_playRect.SetActive(true);
                    m_popupRect.SetActive(false);
                    m_teamSelectRect.SetActive(false);
                    m_spawnPickerRect.SetActive(false);
                    m_stationRect.SetActive(false);
                    break;
                case UIState.Popup:
                    m_pauseRect.SetActive(false);
                    m_playRect.SetActive(false);
                    m_popupRect.SetActive(true);
                    m_teamSelectRect.SetActive(false);
                    m_spawnPickerRect.SetActive(false);
                    m_stationRect.SetActive(false);
                    break;
                case UIState.TeamPicker:
                    m_pauseRect.SetActive(false);
                    m_playRect.SetActive(false);
                    m_popupRect.SetActive(false);
                    m_teamSelectRect.SetActive(true);
                    m_spawnPickerRect.SetActive(false);
                    m_stationRect.SetActive(false);
                    break;
                case UIState.SpawnPicker:
                    m_pauseRect.SetActive(false);
                    m_playRect.SetActive(false);
                    m_popupRect.SetActive(false);
                    m_teamSelectRect.SetActive(false);
                    m_spawnPickerRect.SetActive(true);
                    m_stationRect.SetActive(false);
                    break;
                case UIState.Station:
                    m_pauseRect.SetActive(false);
                    m_playRect.SetActive(false);
                    m_popupRect.SetActive(false);
                    m_teamSelectRect.SetActive(false);
                    m_spawnPickerRect.SetActive(false);
                    m_stationRect.SetActive(true);
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

        #endregion
    }
}