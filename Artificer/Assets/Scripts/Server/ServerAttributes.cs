using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Server
{
    /// <summary>
    /// Stores the attributes used by the server
    /// manager object
    /// </summary>
    public class ServerAttributes : NetworkBehaviour
    {
        #region HUD ELEMENTS

        [Header("HUD Elements")]
        public Transform PlayerList;

        public Transform ServerHUD;

        public Button LeaveBtn;

        public Button StartBtn;

        public Text PlayerCountText;

        public Space.UI.MessageHUD Message;

        #endregion

        #region PREFABS

        [Header("Prefabs")]
        public GameObject PlayerPrefab;

        #endregion

        [Header("Custom Attributes")]
        public int MinPlayers;

        public List<PlayerServerItem> PlayerItems;
    }
}