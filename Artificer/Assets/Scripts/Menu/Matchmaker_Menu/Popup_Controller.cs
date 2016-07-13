using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Menu
{
    /// <summary>
    /// Popup_ controller.
    /// Small object only needs one class
    /// </summary>
    public class Popup_Controller : MonoBehaviour
    {
        [SerializeField]
        private Text Header;
        [SerializeField]
        private Text Msg;

        public void AssignMsg(string[] args)
        {
            Header.text = args[0];
            Msg.text = args[1];
        }

        public void OK()
        {
            GameObject.Find("Online")
                .SendMessage("OpenLobbyListWindow");
        }
    }
}

