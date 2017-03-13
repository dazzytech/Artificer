using UnityEngine;
using System.Collections;
using Steamworks;
using Data.UI;

namespace Menu.Lobby
{
    /// <summary>
    /// Lobby viewer.
    /// Builds info when lobby is selected
    /// </summary>
    public class LobbyViewer : MonoBehaviour
    {
        // List boxes
        [SerializeField]
        private Transform PlayerViewerList;

        [SerializeField]
        private Transform ChatViewerList;

        [SerializeField]
        private Transform SettingsViewerList;

        // Game item prefabs
        [SerializeField]
        private GameObject PlayerCountPrefab;

        [SerializeField]
        private GameObject PlayerItemPrefab;

        [SerializeField]
        private GameObject GameSettingsPrefab;

        private CSteamID m_lobbyID;

        public void AssignLobby(CSteamID newLobby)
        {
            if(newLobby != CSteamID.Nil)
                m_lobbyID = newLobby;
        }

        // refreshes the lobby view
        public void ViewLobby()
        {
            // display players to the right
            ClearPlayers();
            ViewPlayers();

            // display game info
            ClearSettings();
            ViewSettings();
        }

        private void ClearPlayers()
        {
            foreach(Transform child in PlayerViewerList)
            {
                if(child != PlayerViewerList)
                    Destroy(child.gameObject);
            }
        }

        private void ViewPlayers()
        {
            // store the player count in int
            int playerCount = 
                SteamMatchmaking.GetNumLobbyMembers(m_lobbyID);

            // store max amount
            int maxAmount = SteamMatchmaking.GetLobbyMemberLimit(m_lobbyID);

            GameObject PCount = Instantiate(PlayerCountPrefab);
            PCount.transform.SetParent(PlayerViewerList);
            LobbyCountPrefab LC_Prefab = PCount.GetComponent<LobbyCountPrefab>();
            LC_Prefab.SetCounter(playerCount, maxAmount);

            // Iterate through each player and display their name
            for(int i = 0; i < playerCount; i++)
            {
                // retreive player steam ID
                CSteamID OtherPlayer = 
                    SteamMatchmaking.GetLobbyMemberByIndex(m_lobbyID, i);

                // Build player item prefab
                GameObject PlayerItem = Instantiate(PlayerItemPrefab);
                PlayerItem.transform.SetParent(PlayerViewerList);
                LobbyPlayerPrefab P_Prefab = PlayerItem.GetComponent<LobbyPlayerPrefab>();
                P_Prefab.SetPlayer(OtherPlayer, m_lobbyID);
            }
        }

        #region SETTINGS

        /// <summary>
        /// Clears the settings window of all prefabs
        /// </summary>
        private void ClearSettings()
        {
            foreach (Transform child in SettingsViewerList)
            {
                if (child != SettingsViewerList)
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
    }
}