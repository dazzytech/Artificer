using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Space.UI.Pause
{
    public class PauseEventListener : MonoBehaviour
    {
        public GameObject MenuRect;
        public GameObject SettingsRect;
        public GameObject AudioTab;
        public GameObject VideoTab;
        public GameObject ControlTab;

        public void ReturnToGame()
        {
            GameObject.Find("space").SendMessage("Pause", false);
        }

        public void OpenSettingsPanel()
        {
            MenuRect.SetActive(false);
            SettingsRect.SetActive(true);
        }

        public void CloseSettingsPanel()
        {
            MenuRect.SetActive(true);
            SettingsRect.SetActive(false);
        }

        /// <summary>
        /// Switchs to user selected tab.
        /// </summary>
        /// <param name="state">State.</param>
        public void SwitchToTab(string state)
        {
            switch (state)
            {
                case "vid":
                    AudioTab.SetActive(false);
                    VideoTab.SetActive(true);
                    ControlTab.SetActive(false);
                    break;
                case "aud":
                    AudioTab.SetActive(true);
                    VideoTab.SetActive(false);
                    ControlTab.SetActive(false);
                    break;
                case "con":
                    AudioTab.SetActive(false);
                    VideoTab.SetActive(false);
                    ControlTab.SetActive(true);
                    break;
                default:
                    AudioTab.SetActive(false);
                    VideoTab.SetActive(false);
                    ControlTab.SetActive(false);
                    break;
            }
        }

        public void ReturnToMenu()
        {
            GameObject.Find("space").SendMessage("ExitLevel", false);
        }
    }
}
