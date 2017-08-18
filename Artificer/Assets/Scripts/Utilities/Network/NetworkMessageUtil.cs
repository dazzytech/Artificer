using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Space.Projectiles;
using Space.Segment;
using Data.UI;
using Data.Space;
using Data.Space.Collectable;

namespace Networking
{
    #region NETWORK MESSAGE CHANNELS

    enum MSGCHANNEL
    {
        TEAMPICKER = MsgType.Highest + 1,
        NEWID,
        TEAMSELECTED,
        SPAWNPLAYER,
        SPAWNRAIDER,
        SHIPHIT,
        BUILDSTATION,
        PROCESSSHIPHIT,
        OBJECTHIT,
        PROCESSOBJECTHIT,
        SHIPDESTROYED,
        STATIONDESTROYED,
        ASSIGNTEAM,
        INTEGRITYCHANGE,
        DISPLAYINTEGRITYCHANGE,
        LOBBYPLAYERJOINED,
        LOBBYPLAYERLEFT,
        CHATMESSAGESERVER,
        CHATMESSAGECLIENT,
        TRANSACTIONCLIENT,
        TRANSACTIONSERVER,
    };

    #endregion

    #region NETWORK MESSAGE OBJECTS 

    /// <summary>
    /// Message containig the IDs of 
    /// both teams for the client
    /// </summary>
    public class TeamPickerMessage : MessageBase
    {
        public int teamOne;
        public int teamTwo;
    }

    /// <summary>
    /// Message containig the ID of the client
    /// and the team selected
    /// </summary>
    public class TeamSelectionMessage : MessageBase
    {
        public int Selected;
        public int ID;
        public NetworkInstanceId netID;
    }

    /// <summary>
    /// Passes the spawn selection options from player
    /// to the server
    /// </summary>
    public class SpawnSelectionMessage : MessageBase
    {
        public int PlayerID;
        public int SpawnID;
        public ShipData Ship;
    }

    /// <summary>
    /// Passes the requirements to spawn 
    /// </summary>
    public class SpawnRaiderMessage:MessageBase
    {
        public uint PlayerID;
        public uint TargetID;
        /// <summary>
        /// The label of the agent in the list
        /// </summary>
        public string Agent;
    }

    public class StationBuildMessage : MessageBase
    {
        public string PrefabName;
        public Vector3 Position;
        public int teamID;
    }

    /// <summary>
    /// Called when a ship has been hit
    /// to update the server
    /// </summary>
    public class ShipColliderHitMessage : MessageBase
    {
        public int[] HitComponents;
        public float[] HitValues;
        public NetworkInstanceId ShipID;
        public HitData HitD;
    }

    /// <summary>
    /// Called when an object is hit to update the 
    /// Server
    /// </summary>
    public class SOColliderHitMessage : MessageBase
    {
        public NetworkInstanceId SObjectID;
        public HitData HitD;
    }

    /// <summary>
    /// Sent to server when a ship is destroyed
    /// </summary>
    public class ShipDestroyMessage : MessageBase
    {
        public NetworkInstanceId SelfID;
        public int AggressorTeam;
        public int SelfTeam;
        public int ID;
    }

    /// <summary>
    /// Sent to server when station is destroyed
    /// </summary>
    public class StationDestroyMessage : MessageBase
    {
        public NetworkInstanceId SelfID;
        public int ID;
    }

    /// <summary>
    /// Called when a transaction occurs
    /// ingame to update a client of server
    /// </summary>
    public class TransactionMessage : MessageBase
    {
        public int CurrencyDir;

        public int AssetDir;

        /// <summary>
        /// Amount of currency involved in 
        /// transaction 
        /// </summary>
        public int CurrencyAmount;

        /// <summary>
        /// List of items involved in the transaction
        /// </summary>
        public ItemCollectionData[] Assets;

        /// <summary>
        /// Who is receiving the transaction
        /// if -1 assume to be the player
        /// otherwise this is a teamid
        /// </summary>
        public int Recipiant;
    }

    /// <summary>
    /// Used to notify Server and other clients of
    /// a change of integrity either addition or minus
    /// </summary>
    public class IntegrityChangedMsg: MessageBase
    {
        public float Amount;
        public Vector3 Location;
        public int PlayerID;
    }

    /// <summary>
    /// Passes the player count at the time 
    /// of the change as well as the ID of the player
    /// </summary>
    public class LobbyPlayerMsg: MessageBase
    {
        public int PlayerCount;
        public string PlayerName;
    }

    public class ChatParamMsg: MessageBase
    {
        public string style;
        public string messege;
    }

    #endregion

    public class NetworkMessageUtil
    { }
}