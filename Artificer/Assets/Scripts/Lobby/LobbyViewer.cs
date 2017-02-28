using UnityEngine;
using System.Collections;
using Steamworks;

namespace Lobby
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

        CSteamID LobbyID;

        // refreshes the lobby view
        public void ViewLobby(CSteamID pLobby)
        {
            // assign ID
            LobbyID = pLobby;

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
                SteamMatchmaking.GetNumLobbyMembers(LobbyID);

            // store max amount
            int maxAmount = SteamMatchmaking.GetLobbyMemberLimit(LobbyID);

            GameObject PCount = Instantiate(PlayerCountPrefab);
            PCount.transform.SetParent(PlayerViewerList);
            LobbyCount_Prefab LC_Prefab = PCount.GetComponent<LobbyCount_Prefab>();
            LC_Prefab.SetCounter(playerCount, maxAmount);

            // Iterate through each player and display their name
            for(int i = 0; i < playerCount; i++)
            {
                // retreive player steam ID
                CSteamID OtherPlayer = 
                    SteamMatchmaking.GetLobbyMemberByIndex(LobbyID, i);

                // Build player item prefab
                GameObject PlayerItem = Instantiate(PlayerItemPrefab);
                PlayerItem.transform.SetParent(PlayerViewerList);
                LobbyPlayer_Prefab P_Prefab = PlayerItem.GetComponent<LobbyPlayer_Prefab>();
                P_Prefab.SetPlayer(OtherPlayer, LobbyID);
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
            if (SteamMatchmaking.GetLobbyOwner(LobbyID)
                .Equals(SteamUser.GetSteamID()))
                BuildOwnerSettings();
            else
                BuildClientSettings();           
        }

        /// <summary>
        /// Build the setting prefabs with interaction controls
        /// </summary>
        private void BuildOwnerSettings()
        {
            // create variables to store information we want
            string version;             // current game version

            // version label
            version = SteamMatchmaking.GetLobbyData(LobbyID, "ver");

            // create object prefab
            GameObject SettingsItem = Instantiate(GameSettingsPrefab);
            SettingsItem.transform.SetParent(SettingsViewerList);
            LobbySettingsPrefab S_Prefab = SettingsItem.GetComponent<LobbySettingsPrefab>();
            // assign info
            S_Prefab.BuildSettings("Version", version);
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