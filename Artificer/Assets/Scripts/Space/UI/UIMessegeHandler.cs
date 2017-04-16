using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
// Artificer
using Data.Shared;
using Space.UI.Ship;
using Space.UI.Tracker;
using Space.Segment;
using Space.Ship;
using Space.Teams;
using System.Collections.Generic;
using System;
using Space.Ship.Components.Listener;

namespace Space.UI
{
    public enum UIState {Play, Pause, Popup, TeamPicker, SpawnPicker, Station}

    /// <summary>
    /// Redirects incoming messages
    /// from non-UI elements as well was server messages
    /// </summary>
    public class UIMessegeHandler : NetworkBehaviour
    {
        #region ATTRIBUTES

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

        private bool m_keyDelay = false;

        [Header("HUD Additions")]

        [SerializeField]
        private MessageHUD m_messageHUD;

        [Header("Message Docks")]

        [SerializeField]
        private Transform m_playDock;

        [SerializeField]
        private Transform m_pauseDock;

        [SerializeField]
        private Transform m_spawnDock;

        [SerializeField]
        private Transform m_stationDock;

        #endregion

        #region STATE MANAGEMENT

        /// <summary>
        /// Called externally to change the UIState when the game state changes
        /// </summary>
        /// <param name="state"></param>
        public void SetState(UIState state)
        {
            switch (state)
            {
                case UIState.Pause:
                    m_pauseRect.SetActive(true);
                    m_playRect.SetActive(false);
                    m_popupRect.SetActive(false);
                    m_teamSelectRect.SetActive(false);
                    m_spawnPickerRect.SetActive(false);
                    m_messageHUD.gameObject.SetActive(true);
                    m_messageHUD.transform.SetParent(m_pauseDock, false);
                    m_stationRect.SetActive(false);
                    break;
                case UIState.Play:
                    m_pauseRect.SetActive(false);
                    m_playRect.SetActive(true);
                    m_popupRect.SetActive(false);
                    m_teamSelectRect.SetActive(false);
                    m_spawnPickerRect.SetActive(false);
                    m_messageHUD.gameObject.SetActive(true);
                    m_messageHUD.transform.SetParent(m_playDock, false);
                    m_stationRect.SetActive(false);
                    break;
                case UIState.Popup:
                    m_pauseRect.SetActive(false);
                    m_playRect.SetActive(false);
                    m_popupRect.SetActive(true);
                    m_teamSelectRect.SetActive(false);
                    m_spawnPickerRect.SetActive(false);
                    m_messageHUD.gameObject.SetActive(false);
                    m_stationRect.SetActive(false);
                    break;
                case UIState.TeamPicker:
                    m_pauseRect.SetActive(false);
                    m_playRect.SetActive(false);
                    m_popupRect.SetActive(false);
                    m_teamSelectRect.SetActive(true);
                    m_spawnPickerRect.SetActive(false);
                    m_messageHUD.gameObject.SetActive(false);
                    m_stationRect.SetActive(false);
                    break;
                case UIState.SpawnPicker:
                    m_pauseRect.SetActive(false);
                    m_playRect.SetActive(false);
                    m_popupRect.SetActive(false);
                    m_teamSelectRect.SetActive(false);
                    m_spawnPickerRect.SetActive(true);
                    m_messageHUD.gameObject.SetActive(true);
                    m_messageHUD.transform.SetParent(m_spawnDock, false);
                    m_stationRect.SetActive(false);
                    break;
                case UIState.Station:
                    m_pauseRect.SetActive(false);
                    m_playRect.SetActive(false);
                    m_popupRect.SetActive(false);
                    m_teamSelectRect.SetActive(false);
                    m_spawnPickerRect.SetActive(false);
                    m_messageHUD.gameObject.SetActive(true);
                    m_messageHUD.transform.SetParent(m_stationDock, false);
                    m_stationRect.SetActive(true);
                    break;
            }
        }

        #endregion

        #region PLAYRECT UI MESSAGES

        /// <summary>
        /// Prompts the GUI to retreive ship
        /// attributes and update the HUD
        /// </summary>
        public void BuildShipData()
        {
            m_playRect.GetComponent<PlayHUD>().
                BuildShipData();
        }

        /// <summary>
        /// Redraw the HUD
        /// </summary>
        public void RebuildGUI()
        {
            m_playRect.SendMessage("RebuildGUI");
        }

        /// <summary>
        /// Sends message to the UI text towards the 
        /// bottom of the screen
        /// </summary>
        /// <param name="message"></param>
        public void DisplayPrompt(string message)
        {
            if (m_messageHUD.gameObject.activeSelf)
            {
                m_messageHUD.DisplayPrompt(message);
            }
        }

        /// <summary>
        /// Clears the text at the bottom of the screen
        /// </summary>
        public void ClearPrompt()
        {
            if (m_messageHUD.gameObject.activeSelf)
            {
                m_messageHUD.HidePrompt();
            }
        }

        /// <summary>
        /// Adds a peice for the TrackerHUD to track
        /// a transform dependant on its tag
        /// </summary>
        /// <param name="piece"></param>
        public void AddUIPiece(Transform piece)
        {
            m_playRect.GetComponent<TrackerHUD>().
                AddUIPiece(piece);
        }

        public void DisplayIntegrityChange
            (Vector2 postion, float amount)
        {
            m_playRect.GetComponent<IndicatorHUD>()
                .IndicateIntegrity(postion, amount);
        }

        /// <summary>
        /// displays construction
        /// heads up display
        /// </summary>
        /// <param name="deployFunction"></param>
        /// <param name="options"></param>
        public void DisplayBuildWheel(Deploy deployFunction, 
            List<string> options)
        {

        }

        #endregion

        #region PLAYRECT RPC

        [ClientRpc]
        public void RpcAddRemotePlayer(NetworkInstanceId instID)
        {
            // retreive our version of that network instance 
            GameObject otherPlayer = ClientScene.FindLocalObject(instID);

            // if this is our ship we dont want to add it to UI
            if (otherPlayer.GetComponent<NetworkIdentity>().isLocalPlayer)
                return;

            // Add to UI
            AddUIPiece(otherPlayer.transform);
        }

        #endregion

        #region POPUP UI MESSAGES

        /// <summary>
        /// Sets the value for the ship spawn counter 
        /// and triggers the countdown
        /// </summary>
        /// <param name="val"></param>
        public void SetCounter(float val)
        {
            m_popupRect.SendMessage("SetShipRespawnCounter", val);
        }

        /// <summary>
        /// Displays endgame display with boolean that
        /// states if win
        /// </summary>
        /// <param name="val"></param>
        public void EndGame(bool val)
        {
            m_popupRect.SendMessage("EndGame", val);
        }

        /*public void UpdateReward(object val)
        {
            PopupRect.SendMessage("UpdateReward", val);
        }*/

        #endregion

        #region TEAM PICKER MESSAGES
            
        /// <summary>
        /// Sets the team picker rect to show 
        /// which team objects can be chosen
        /// </summary>
        /// <param name="teamA"></param>
        /// <param name="teamB"></param>
        public void SetTeamOptions(int teamA, int teamB)
        {
            m_teamSelectRect.SendMessage("SetTeams", 
                new int[2] { teamA, teamB });
        }

        #endregion

        #region SPAWN PICKER MESSAGES

        /// <summary>
        /// Set the HUD spawn button to enable after a certain period
        /// </summary>
        /// <param name="delay"></param>
        public void SetSpawnDelay(int delay)
        {
            m_spawnPickerRect.SendMessage("SetSpawnTimer", delay);
        }

        #endregion

        #region INTERACTIVE

        /// <summary>
        /// Sets the HUD visible or not if on playHUD
        /// </summary>
        public void ToggleHUD()
        {
            if (!m_keyDelay && !m_popupRect.activeSelf)
            {
                m_playRect.SetActive(!m_playRect.activeSelf);
                m_keyDelay = true;
                Invoke("PauseRelease", 0.3f);
            }
        }

        /// <summary>
        /// The player can set the mission HUD visible and invisible alone
        /// due to its size
        /// </summary>
        public void ToggleStationHUD()
        {
            if(!m_popupRect.activeSelf && m_playRect.activeSelf)
                m_playRect.SendMessage("HidePanel", "station");
        }

        public void PauseRelease()
        {
            m_keyDelay = false;
        }

        #endregion

        #region STATION RECT MESSAGES

        /// <summary>
        /// prompts the station object to enable
        /// the warp map portion of the Station HUD
        /// </summary>
        public void InitializeWarpMap
            (List<NetworkInstanceId> surroundingWarpGates)
        {
            m_stationRect.SendMessage("InitializeWarpMap", surroundingWarpGates);
        }

        /// <summary>
        /// Prompts the staton object to enable the ship
        /// viewer portion of the station HUD. 
        /// </summary>
        /// <param name="ship"></param>
        public void InitializeStationHUD(ShipAttributes ship)
        {
            m_stationRect.SendMessage("InitializeShipViewer", ship);
        }

        #endregion

        /// <summary>
        /// Updates the chat window with a message
        /// sent within a MsgParam
        /// </summary>
        /// <param name="param"></param>
        public void DisplayMessege(MsgParam param)
        {
            m_messageHUD.DisplayMessege(param);
        }
    }
}
