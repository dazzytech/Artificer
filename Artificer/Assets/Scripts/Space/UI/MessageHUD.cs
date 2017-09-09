using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

using UI.Effects;
using Networking;

namespace Space.UI
{
    public class MessageHUD: MonoBehaviour
    {
        private static MessageHUD instance = null;

        [SerializeField]
        private Transform HUD;

        private static int messegeCount = 0;
        private static int messegeMax = 20;
        
        public Font font;

        #region MONO BEHAVIOUR

        void Awake()
        {
            messegeCount = 0;
            messegeMax = 20;

            NetworkManager.singleton.client.RegisterHandler((short)MSGCHANNEL.CHATMESSAGECLIENT, DisplayMessege);
        }

        #endregion

        #region PUBLIC MESSAGES

        /// <summary>
        /// Display a message to the chat window
        /// with formatting
        /// </summary>
        /// <param name="param"></param>
        public void DisplayMessege(NetworkMessage msg)
        {
            ChatParamMsg param = msg.ReadMessage<ChatParamMsg>();

            GameObject disp = new GameObject();
            Text text = disp.AddComponent<Text>();
            //disp.transform.localScale = new Vector3(1f, 1f, 1f);
            //disp.transform.localPosition = new Vector3(0f, 0f, 0f);
            disp.transform.SetParent(HUD, false);

                switch (param.style)
                {
                    case "small":
                        text.color = new Color(1, 1, 1, 1);
                        text.fontSize = 8;
                        text.alignment = TextAnchor.UpperLeft;
                        break;
                    case "sm-red":
                        text.color = new Color(1, .1f, .1f, 1);
                        text.fontSize = 8;
                        text.alignment = TextAnchor.UpperLeft;
                        break;
                    case "sm-green":
                        text.color = new Color(.2f, 1, .1f, 1);
                        text.fontSize = 8;
                        text.alignment = TextAnchor.UpperLeft;
                        break;
                    case "md-green":
                        text.color = new Color(0.2f, 1, .1f, 1);
                        text.fontSize = 10;
                        text.alignment = TextAnchor.UpperLeft;
                        break;
                    case "md-yellow":
                        text.color = new Color(0.5f, .5f, .1f, 1);
                        text.fontSize = 10;
                        text.alignment = TextAnchor.UpperLeft;
                        break;
                    case "md-red":
                        text.color = new Color(1f, .1f, .1f, 1);
                        text.fontSize = 10;
                        text.alignment = TextAnchor.UpperLeft;
                        break;
                    case "bold":
                        text.color = new Color(.6f, 1f, .6f, 1f);
                        text.fontSize = 10;
                        text.fontStyle = FontStyle.Bold;
                        text.alignment = TextAnchor.UpperLeft;
                        break;
                }

            text.font = This.font;
            text.text = param.messege;
            messegeCount++;

            // clear top messege if count reaches max
            if (messegeCount > messegeMax)
            {
                for(int i = 0; i < 5; i++)
                {
                    GameObject.Destroy(HUD.GetChild(i).gameObject);
                    messegeCount--;
                }
            }

        }

        #endregion

        #region ACCESSORS

        private MessageHUD This
        {
            get
            {
                if (instance == null)
                    instance = this;

                return instance;
            }
        }

        #endregion
    }
}
