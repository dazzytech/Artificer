using UnityEngine;
using System.Collections;
using Steamworks;
using Data.Space;
using System.Collections.Generic;
using Data.Shared;

namespace Data.UI
{
    /// <summary>
    /// Container object for a player reference 
    /// within a server. Not for use ingame
    /// In steam matchmaker this is built from 
    /// retriving info from SteamID
    /// </summary>
    [System.Serializable]
    public struct PlayerData
    {
        #region PLAYER INFORMATION

        // Public name (steam name)
        public string PlayerName;

        // public avatar (Steam avatar)
        //public Texture2D PlayerAvatar;

        public int PlayerID;

        public bool IsHost;

        #endregion

        #region PLAYER STATISTICS

        // Not yet implemented (most likely incl level)

        #endregion

        #region PLAYER INVENTORY
        
        /// <summary>
        /// Ships that the player has access to
        /// (prebuilt and custom)
        /// </summary>
        public ShipSpawnData[] ShipInventory;

        /// <summary>
        /// Cash and item assets the player 
        /// owns
        /// </summary>
        public WalletData Wallet;

        /// <summary>
        /// NPCs that the player has created.
        /// </summary>
        public NPCPrefabData[] NPCPrefabs;

        #endregion
    }
}