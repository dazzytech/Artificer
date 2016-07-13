using UnityEngine;
using System.Collections;
// Artificer
using Data.Shared;
using Space.UI.Ship;
using Space.UI.Tracker;

namespace Space.UI
{
    public enum UIState {Play, Pause, Popup}

    public class UIMessegeHandler : MonoBehaviour
    {
        public GameObject PlayRect;
        public GameObject PauseRect;
        public GameObject PopupRect;
        private bool _keyDelay = false;

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

        public void BuildShipData()
        {
            PlayRect.GetComponent<ShipHUD>().
                BuildShipData();
        }

        public void BuildContractData(ContractData att)
        {
            PlayRect.GetComponent<ShipHUD>().BuildContractData(att);
        }

        public void AddUIPiece(Transform piece)
        {
            PlayRect.GetComponent<TrackerHUD>().
                AddUIPiece(piece);
        }

        public void RebuildGUI()
        {
            PlayRect.SendMessage("RebuildGUI");
        }

        public void DisplayMessege(MsgParam param)
        {
            PlayRect.SendMessage("DisplayMessege", param);
        }

        // popup func
        public void SetCounter(float val)
        {
            PopupRect.SendMessage("SetShipRespawnCounter", val);
        }

        public void EndGame(bool val)
        {
            PopupRect.SendMessage("EndGame", val);
        }

        public void UpdateReward(object val)
        {
            PopupRect.SendMessage("UpdateReward", val);
        }

        public void ToggleHUD()
        {
            if (!_keyDelay && !PopupRect.activeSelf)
            {
                PlayRect.SetActive(!PlayRect.activeSelf);
                _keyDelay = true;
                Invoke("PauseRelease", 0.3f);
            }
        }

        public void ToggleMissionHUD()
        {
            if(!PopupRect.activeSelf && PlayRect.activeSelf)
                PlayRect.SendMessage("ToggleHUD");
        }

        public void PauseRelease()
        {
            _keyDelay = false;
        }
    }   
}
