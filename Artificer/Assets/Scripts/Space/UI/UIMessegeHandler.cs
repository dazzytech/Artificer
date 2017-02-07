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

        public GameObject PlayRect;
        public GameObject PauseRect;
        public GameObject PopupRect;
        public GameObject TeamSelectRect;
        public GameObject SpawnPickerRect;
        public GameObject StationRect;
        private bool _keyDelay = false;

        // messages
        public MessageHUD MsgBase;

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
                    PauseRect.SetActive(true);
                    PlayRect.SetActive(false);
                    PopupRect.SetActive(false);
                    TeamSelectRect.SetActive(false);
                    SpawnPickerRect.SetActive(false);
                    MsgBase.gameObject.SetActive(false);
                    StationRect.SetActive(false);
                    break;
                case UIState.Play:
                    PauseRect.SetActive(false);
                    PlayRect.SetActive(true);
                    PopupRect.SetActive(false);
                    TeamSelectRect.SetActive(false);
                    SpawnPickerRect.SetActive(false);
                    MsgBase.gameObject.SetActive(true);
                    StationRect.SetActive(false);
                    break;
                case UIState.Popup:
                    PauseRect.SetActive(false);
                    PlayRect.SetActive(false);
                    PopupRect.SetActive(true);
                    TeamSelectRect.SetActive(false);
                    SpawnPickerRect.SetActive(false);
                    MsgBase.gameObject.SetActive(true);
                    StationRect.SetActive(false);
                    break;
                case UIState.TeamPicker:
                    PauseRect.SetActive(false);
                    PlayRect.SetActive(false);
                    PopupRect.SetActive(false);
                    TeamSelectRect.SetActive(true);
                    SpawnPickerRect.SetActive(false);
                    MsgBase.gameObject.SetActive(false);
                    StationRect.SetActive(false);
                    break;
                case UIState.SpawnPicker:
                    PauseRect.SetActive(false);
                    PlayRect.SetActive(false);
                    PopupRect.SetActive(false);
                    TeamSelectRect.SetActive(false);
                    SpawnPickerRect.SetActive(true);
                    MsgBase.gameObject.SetActive(false);
                    StationRect.SetActive(false);
                    break;
                case UIState.Station:
                    PauseRect.SetActive(false);
                    PlayRect.SetActive(false);
                    PopupRect.SetActive(false);
                    TeamSelectRect.SetActive(false);
                    SpawnPickerRect.SetActive(false);
                    MsgBase.gameObject.SetActive(true);
                    StationRect.SetActive(true);
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
            PlayRect.GetComponent<PlayHUD>().
                BuildShipData();
        }

        /// <summary>
        /// Redraw the HUD
        /// </summary>
        public void RebuildGUI()
        {
            PlayRect.SendMessage("RebuildGUI");
        }

        /// <summary>
        /// Updates the chat window with a message
        /// sent within a MsgParam
        /// </summary>
        /// <param name="param"></param>
        public void DisplayMessege(MsgParam param)
        {
            //PlayRect.SendMessage("DisplayMessege", param);
            if(MsgBase.gameObject.activeSelf)
            {
                MsgBase.DisplayMessege(param);
            }
        }

        /// <summary>
        /// Sends message to the UI text towards the 
        /// bottom of the screen
        /// </summary>
        /// <param name="message"></param>
        public void DisplayPrompt(string message)
        {
            if (MsgBase.gameObject.activeSelf)
            {
                MsgBase.DisplayPrompt(message);
            }
        }

        /// <summary>
        /// Clears the text at the bottom of the screen
        /// </summary>
        public void ClearPrompt()
        {
            if (MsgBase.gameObject.activeSelf)
            {
                MsgBase.HidePrompt();
            }
        }

        /// <summary>
        /// Adds a peice for the TrackerHUD to track
        /// a transform dependant on its tag
        /// </summary>
        /// <param name="piece"></param>
        public void AddUIPiece(Transform piece)
        {
            PlayRect.GetComponent<TrackerHUD>().
                AddUIPiece(piece);
        }

        public void DisplayIntegrityChange
            (Vector2 postion, float amount)
        {
            PlayRect.GetComponent<IndicatorHUD>()
                .IndicateIntegrity(postion, amount);
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
            PopupRect.SendMessage("SetShipRespawnCounter", val);
        }

        /// <summary>
        /// Displays endgame display with boolean that
        /// states if win
        /// </summary>
        /// <param name="val"></param>
        public void EndGame(bool val)
        {
            PopupRect.SendMessage("EndGame", val);
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
            TeamSelectRect.SendMessage("SetTeams", 
                new int[2] { teamA, teamB });
        }

        #endregion

        #region SPAWN PICKER MESSAGES

        /// <summary>
        /// Set the HUD spawn button to enable after a certain period
        /// </summary>
        /// <param name="delay"></param>
        public void SetSpawnDelay(float delay)
        {
            SpawnPickerRect.SendMessage("SetSpawnTimer", delay);
        }

        #endregion

        #region INTERACTIVE

        /// <summary>
        /// Sets the HUD visible or not if on playHUD
        /// </summary>
        public void ToggleHUD()
        {
            if (!_keyDelay && !PopupRect.activeSelf)
            {
                PlayRect.SetActive(!PlayRect.activeSelf);
                _keyDelay = true;
                Invoke("PauseRelease", 0.3f);
            }
        }

        /// <summary>
        /// The player can set the mission HUD visible and invisible alone
        /// due to its size
        /// </summary>
        public void ToggleStationHUD()
        {
            if(!PopupRect.activeSelf && PlayRect.activeSelf)
                PlayRect.SendMessage("HidePanel", "station");
        }

        public void PauseRelease()
        {
            _keyDelay = false;
        }

        #endregion

        #region STATION RECT MESSAGES

        public void InitializeStationHUD(ShipAttributes ship)
        {
            StationRect.SendMessage("InitializeHUD", ship);
        }

        #endregion
    }
}
