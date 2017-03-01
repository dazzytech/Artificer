using UnityEngine;
using System.Collections;
using Steamworks;

namespace Data.UI
{
    /// <summary>
    /// Container object for a player reference 
    /// within a server. Not for use ingame
    /// In steam matchmaker this is built from 
    /// retriving info from SteamID
    /// </summary>
    public struct PlayerData
    {
        #region PLAYER INFORMATION

        // Public name (steam name)
        public string PlayerName;

        // public avatar (Steam avatar)
        //public Texture2D PlayerAvatar;

        public int PlayerID;

        #endregion

        #region PLAYER STATISTICS

        // Not yet implemented (most likely incl level)

        #endregion
    }
}