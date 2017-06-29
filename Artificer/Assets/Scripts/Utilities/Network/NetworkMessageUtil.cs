using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Space.Projectiles;
using Space.Segment;
using Data.UI;
using Data.Space;

namespace Networking
{
    #region NETWORK MESSAGE CHANNELS

    enum MSGCHANNEL
    {
        TEAMPICKER = MsgType.Highest + 1,
        NEWID,
        TEAMSELECTED,
        SPAWNPLAYER,
        SPAWNAI,
        SHIPHIT,
        CREATEPROJECTILE,
        BUILDPROJECTILE,
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
        public string ShipName;
    }

    /// <summary>
    /// Information sent to the 
    /// server to spawn an ai agent
    /// </summary>
    public class SpawnAIMessage : MessageBase
    {
        public int ID;
        public AgentData Agent;
        public Vector2 Point;
    }

    /// <summary>
    /// Request from player to server to 
    /// spawn a projectile with local player
    /// authority
    /// </summary>
    public class ProjectileBuildMessage : MessageBase
    {
        public Vector3 Position;
        public int PrefabIndex, shooterID;
        public WeaponData WData;
    }

    public class StationBuildMessage : MessageBase
    {
        public string PrefabName;
        public Vector3 Position;
        public int teamID;
    }

    /// <summary>
    /// Message sent when projectile is spawned by server
    /// with reference for accessing new projectile
    /// </summary>
    public class ProjectileSpawnedMessage : MessageBase
    {
        public NetworkInstanceId Projectile;
        public WeaponData WData;
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
        public string AggressorTag;
        public string AlignmentLabel;
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
    /// Sent to each client to add stations to HUD
    /// </summary>
    public class NetMsgMessage : MessageBase
    {
        public NetworkInstanceId SelfID;
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