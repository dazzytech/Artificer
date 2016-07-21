using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
// Artificer
using Data.Shared;
using Space.UI.Ship;
using Space.UI.Tracker;

namespace Space.UI
{
    public enum UIState {Play, Pause, Popup}

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
        private bool _keyDelay = false;

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
                    break;
                case UIState.Play:
                    PauseRect.SetActive(false);
                    PlayRect.SetActive(true);
                    PopupRect.SetActive(false);
                    break;
                case UIState.Popup:
                    PauseRect.SetActive(false);
                    PlayRect.SetActive(false);
                    PopupRect.SetActive(true);
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
            PlayRect.GetComponent<ShipHUD>().
                BuildShipData();
        }

        /*public void BuildContractData(ContractData att)
        {
            PlayRect.GetComponent<ShipHUD>().BuildContractData(att);
        }*/

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
            PlayRect.SendMessage("DisplayMessege", param);
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
        public void ToggleMissionHUD()
        {
            if(!PopupRect.activeSelf && PlayRect.activeSelf)
                PlayRect.SendMessage("ToggleHUD");
        }

        public void PauseRelease()
        {
            _keyDelay = false;
        }

        #endregion
    }
}
