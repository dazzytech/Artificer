using Data.UI;
using Networking;
using Space.UI.Prompt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space.UI
{
    public class UIMessageController : MonoBehaviour
    {
        #region ATTRIBUTES

        [SerializeField]
        private MessageHUD m_messageHUD;

        [SerializeField]
        private MessagePromptHUD m_promptHUD;

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

        #region PUBLIC INTERACTION

        /// <summary>
        /// Creates a message parameter 
        /// and sends message to server
        /// </summary>
        /// <param name="param"></param>
        public void DisplayMessege(string style, string text)
        {
            ChatParamMsg msg = new ChatParamMsg();
            msg.style = style;
            msg.messege = text;

            SystemManager.singleton.client.Send((short)MSGCHANNEL.CHATMESSAGESERVER, msg);
        }

        /// <summary>
        /// Sends message to the UI text towards the 
        /// bottom of the screen
        /// </summary>
        public void DisplayPrompt(PromptData message)
        {
            // Create a prompt text class?
            if (m_promptHUD.gameObject.activeSelf)
            {
                m_promptHUD.DisplayPrompt(message);
            }
        }

        /// <summary>
        /// Clears the text at the bottom of the screen
        /// </summary>
        public void ClearPrompt(int index)
        {
            if (m_promptHUD.gameObject.activeSelf)
            {
                // change to manual removal
                m_promptHUD.HidePrompt(index);
            }
        }

        public void SetState(UIState state)
        {
            switch (state)
            {
                case UIState.Pause:
                    m_messageHUD.gameObject.SetActive(true);
                    m_messageHUD.transform.SetParent(m_pauseDock, false);
                    break;
                case UIState.Play:
                    m_messageHUD.gameObject.SetActive(true);
                    m_messageHUD.transform.SetParent(m_playDock, false);
                    break;
                case UIState.SpawnPicker:
                    m_messageHUD.gameObject.SetActive(true);
                    m_messageHUD.transform.SetParent(m_spawnDock, false);
                    break;
                case UIState.Station:
                    m_messageHUD.gameObject.SetActive(true);
                    m_messageHUD.transform.SetParent(m_stationDock, false);
                    break;
                default:
                    m_messageHUD.gameObject.SetActive(false);
                    break;
            }
        }

        #endregion
    }
}
