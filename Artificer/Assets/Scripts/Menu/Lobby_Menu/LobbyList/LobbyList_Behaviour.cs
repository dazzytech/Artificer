using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Steamworks;

namespace Menu
{
    /// <summary>
    /// Lobby_ behaviour.
    /// builds the lobby list upon opening the lobby screen
    /// </summary>
    public class LobbyList_Behaviour : MonoBehaviour 
    {
        // Create delegate to manage lobby selection
        public delegate void selectLobby(CSteamID pLobby);

        private void SelectLobby(CSteamID lobby)
        {
            m_Lobby = lobby;
            OnSelectLobby(lobby);
        }

        //Create event for other objects to listen to
        public event selectLobby OnSelectLobby;

        //public event SelectLobby OnSelectLobby;

        // SteamUsername Vars
        private string public_alias;
        [SerializeField]
        private Text AliasText;
        [SerializeField]
        private GameObject LobbyItemPrefab;
        [SerializeField]
        private Transform LobbyList;

        // Lobby Vars
        public CSteamID m_Lobby;

        // Store filter Items
        public FilterInfo Filter;


    	// Use this for initialization
    	void Start () 
        {
            Filter = null;

            if(SteamManager.Initialized)
            {
                public_alias = SteamFriends.GetPersonaName();
                if(AliasText  != null) AliasText.text = public_alias;
            }
        }

        public void BuildLobbyItem(CSteamID pLobby)
        {
            GameObject Lobby = Instantiate(LobbyItemPrefab);
            Lobby.transform.SetParent(LobbyList);

            //Get functionality
            Lobby_Prefab LPrefab = Lobby.GetComponent<Lobby_Prefab>();

            LPrefab.StartCoroutine(LPrefab.InitializeLobby(pLobby,
                           new selectLobby(SelectLobby)));
        }

        public void ClearLobby()
        {
            foreach(Transform child in LobbyList)
            {
                if(child != LobbyList)
                    Destroy(child.gameObject);
            }
        }

        public CSteamID ReturnLobby()
        {
            return m_Lobby;
        }
    }
}
