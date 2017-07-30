using UnityEngine;
using System.Collections;
using Steamworks;
using Data.UI;
using UnityEngine.UI;

namespace Menu.Lobby
{
    /// <summary>
    /// Lobby viewer.
    /// Builds info when lobby is selected
    /// </summary>
    public class LobbyViewer : HUDPanel
    {
        #region ATTRIBUTES

        [SerializeField]
        private LobbyAttributes m_att;

        #region HUD ELEMENTS

        [Header("HUD Elements")]

        [SerializeField]
        private Transform m_playerViewerList;

        [SerializeField]
        private Transform m_settingsViewerList;

        [SerializeField]
        private Text m_playerCounter;

        #endregion

        #region PREFABS

        [Header("Prefabs")]

        [SerializeField]
        private GameObject m_playerItemPrefab;

        [SerializeField]
        private GameObject m_gameSettingsPrefab;

        #endregion

        #endregion

        #region ACCESSOR

        private CSteamID LobbyID
        {
            get { return m_att.CurrentLobby.GetID; }
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Reset settings viewer
        /// when Lobby data is changed
        /// </summary>
        public void UpdateSettings()
        {
            // display game info
            ClearSettings();
            ViewSettings();
        }

        /// <summary>
        /// Resets player viewer when player
        /// state is changed
        /// </summary>
        public void UpdatePlayers()
        {
            // display players to the right
            ClearPlayers();
            ViewPlayers();
        }

        #endregion

        #region PRIVATE UTILITIES

        #region PLAYERS

        /// <summary>
        /// Refresh the lobby player list
        /// </summary>
        private void ClearPlayers()
        {
            foreach(Transform child in m_playerViewerList)
            {
                if(child != m_playerViewerList)
                    Destroy(child.gameObject);
            }
        }

        /// <summary>
        /// Build the player list in lobby
        /// viewer
        /// </summary>
        private void ViewPlayers()
        {
            // store the player count in int
            int playerCount = 
                SteamMatchmaking.GetNumLobbyMembers(LobbyID);

            // store max amount
            int maxAmount = SteamMatchmaking.GetLobbyMemberLimit(LobbyID);

            // Display the current player count
            m_playerCounter.text = string.Format
                ("Player Count: {0} | Player Limit: {1}",
                playerCount, maxAmount);

            // Iterate through each player and display their name
            for(int i = 0; i < playerCount; i++)
            {
                // retreive player steam ID
                CSteamID OtherPlayer = 
                    SteamMatchmaking.GetLobbyMemberByIndex(LobbyID, i);

                // Build player item prefab
                GameObject PlayerItem = Instantiate(m_playerItemPrefab);
                PlayerItem.transform.SetParent(m_playerViewerList, false);
                PlayerItem.transform.localScale = new Vector3(1, 1, 1);
                LobbyPlayerPrefab P_Prefab = PlayerItem.GetComponent<LobbyPlayerPrefab>();
                P_Prefab.SetPlayer(OtherPlayer, LobbyID);
            }
        }

        #endregion

        #region SETTINGS

        /// <summary>
        /// Clears the settings window of all prefabs
        /// </summary>
        private void ClearSettings()
        {
            foreach (Transform child in m_settingsViewerList)
            {
                if (child != m_settingsViewerList)
                    Destroy(child.gameObject);
            }
        }

        /// <summary>
        /// Finds out if we are the owner and invokes the related function
        /// </summary>
        private void ViewSettings()
        {
            // Check if we are the lobby owner
            /*if (SteamMatchmaking.GetLobbyOwner(m_lobbyID)
                .Equals(SteamUser.GetSteamID()))
                BuildOwnerSettings();
            else
                BuildClientSettings();   */        
        }

        /// <summary>
        /// Build the setting prefabs with interaction controls
        /// </summary>
        private void BuildOwnerSettings()
        {
            // create variables to store information we want
            /*string version;             // current game version

            // version label
            version = SteamMatchmaking.GetLobbyData(m_lobbyID, "ver");

            // create object prefab
            GameObject SettingsItem = Instantiate(GameSettingsPrefab);
            SettingsItem.transform.SetParent(SettingsViewerList);
            LobbySettingsPrefab S_Prefab = SettingsItem.GetComponent<LobbySettingsPrefab>();
            // assign info
            S_Prefab.BuildSettings("Version", version);*/
        }

        /// <summary>
        /// Build the setting prefabs without interaction controls
        /// </summary>
        private void BuildClientSettings()
        {
            // dont do anything yet
        }

        #endregion

        #endregion
    }
}