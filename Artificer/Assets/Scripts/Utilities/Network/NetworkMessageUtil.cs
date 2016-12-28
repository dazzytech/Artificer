using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Space.Projectiles;
using Space.Segment;

namespace Networking
{
    #region NETWORK MESSAGE CHANNELS

    enum MSGCHANNEL
    {
        TEAMPICKER = MsgType.Highest + 1,
        NEWID,
        ASSIGNTOTEAM,
        SPAWNPLAYER,
        SPAWNME,
        SHIPHIT,
        CREATEPROJECTILE,
        BUILDPROJECTILE,
        PROCESSSHIPHIT,
        OBJECTHIT,
        PROCESSOBJECTHIT,
        SHIPDESTROYED,
        STATIONDESTROYED,
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
        public int ShipID;
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
    }

    /// <summary>
    /// Sent to server when station is destroyed
    /// </summary>
    public class StationDestroyMessage : MessageBase
    {
        public NetworkInstanceId SelfID;
        public int ID;
    }

    #endregion


    public class NetworkMessageUtil
    { }
}