using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using Steamworks;

namespace Menu.Lobby
{
    //Displays player info to the panel
    public class LobbyPlayerPrefab : MonoBehaviour 
    {
        [SerializeField]
        private Text PlayerName;
        [SerializeField]
        private RawImage PlayerIcon;
        [SerializeField]
        private RawImage PlayerStatus;
        [SerializeField]
        private Text PlayerStatusPrompt;
        [SerializeField]
        private Button PlayerStatusButton;
        [SerializeField]
        private Button PlayerKickButton;

        // used be local user to update data
        private CSteamID LobbyID;

        [SerializeField]
        private Texture2D PlayerYes;
        [SerializeField]
        private Texture2D PlayerNo;

        /// <summary>
        /// Displays the player's status
        /// including ready status and info
        /// stats to the small panel
        /// </summary>
        /// <param name="playerID"></param>
        /// <param name="lobbyID"></param>
        public void SetPlayer(CSteamID playerID, CSteamID lobbyID)
        {
            Debug.Log("Building Player");
            string name = SteamFriends.GetFriendPersonaName(playerID);
            PlayerName.text = name;

            // If not lobby owner then remove kick button
            if (SteamUser.GetSteamID() != SteamMatchmaking.GetLobbyOwner(lobbyID))
            {
                PlayerKickButton.gameObject.SetActive(false);
            }

            // Retreive and post avatar
            int FriendAvatar = SteamFriends.GetMediumFriendAvatar(playerID);

            uint ImageWidth;
            uint ImageHeight;
            bool ret = SteamUtils.GetImageSize(FriendAvatar, out ImageWidth, out ImageHeight);

            if (ret && ImageWidth > 0 && ImageHeight > 0)
            {
                byte[] Image = new byte[4 * ImageWidth * ImageHeight * sizeof(char)];

                ret = SteamUtils.GetImageRGBA(FriendAvatar, Image, (int)(4 * ImageWidth * ImageHeight * sizeof(char)));

                Texture2D Avatar = new Texture2D((int)ImageWidth, (int)ImageHeight, TextureFormat.RGBA32, false, true);
                Avatar.LoadRawTextureData(Image);
                Avatar.Apply();

                PlayerIcon.texture = Avatar;
            }

            // We only want to perform this if lobby is public type
            if (SteamMatchmaking.GetLobbyData(lobbyID, "live") == "true")
            {
                // Display player ready status
                string readyStatus = SteamMatchmaking.GetLobbyMemberData(lobbyID, playerID, "ready");
                if (readyStatus == "true")
                {
                    PlayerStatus.texture = PlayerYes;
                    PlayerStatusPrompt.text = "Set Not Ready";
                }
                else
                {
                    PlayerStatus.texture = PlayerNo;
                    PlayerStatusPrompt.text = "Set Ready";
                }

            }
            else
            {
                // we dont want to set ready toggle if we are in an invisible lobby

                // hide button;
                PlayerStatusButton.gameObject.SetActive(false);

                // hide indicator
                PlayerStatus.gameObject.SetActive(false);
            }

            // Active Status
            if (SteamUser.GetSteamID() == playerID)
            {
                // Can't kick ourselves
                PlayerKickButton.gameObject.SetActive(false);
            }

            // kick button

            // assign vars
            LobbyID = lobbyID;
        }

        public void ToggleStatus()
        {
            // update our state
            bool bOldState = (SteamMatchmaking.GetLobbyMemberData(LobbyID, SteamUser.GetSteamID(), "ready") == "true");
            bool bNewState = !bOldState;

            Debug.Log(bNewState);

            // publish to everyone
            SteamMatchmaking.SetLobbyMemberData(LobbyID, "ready", bNewState ? "true" : "false");
        }

        public void KickPlayer()
        {
            // nothing here yet

        }
    }
}
