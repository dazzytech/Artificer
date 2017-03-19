using UnityEngine;
using UnityEngine.UI;
using Steamworks;

namespace Menu.Lobby
{
    /// <summary>
    /// Displays a lobby player visually
    /// </summary>
    public class LobbyPlayerPrefab : MonoBehaviour 
    {
        #region ATTRIBUTES

        [Header("HUD Elements")]

        [SerializeField]
        private Text PlayerName;
        [SerializeField]
        private RawImage PlayerIcon;
        
        // used be local user to update data
        private CSteamID LobbyID;

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Build the players visual elements
        /// e.g. icon and name
        /// </summary>
        /// <param name="playerID"></param>
        /// <param name="lobbyID"></param>
        public void SetPlayer(CSteamID playerID, CSteamID lobbyID)
        { 
            string name = SteamFriends.GetFriendPersonaName(playerID);
            PlayerName.text = name;

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

            // assign vars
            LobbyID = lobbyID;
        }

        #endregion
    }
}
