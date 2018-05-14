using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

using Networking;

namespace Game
{
    /// <summary>
    /// dispatches events that are to trigger 
    /// other server listeners
    /// </summary>
    public class GameServerEvents : NetworkBehaviour
    {
        #region EVENTS

        public delegate void CreatedEvent(CreateDispatch CD);
        [SyncEvent]
        public event CreatedEvent EventShipCreated;
        [SyncEvent]
        public event CreatedEvent EventStationCreated;

        public delegate void DestroyedEvent(DestroyDespatch DD);
        [SyncEvent]
        public event DestroyedEvent EventShipDestroyed;
        [SyncEvent]
        public event DestroyedEvent EventStationDestroyed;
        

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Sends an event across the server
        /// that the ship has been created
        /// </summary>
        /// <param name="msg"></param>
        [Server]
        public void ShipCreated(NetworkInstanceId Self, int ID, int Align)
        {
            CreateDispatch CD = new CreateDispatch();
            CD.PlayerID = ID;
            CD.Self = Self.Value;
            CD.TeamID = Align;
            EventShipCreated(CD);
        }

        /// <summary>
        /// Sends across the server that 
        /// a ship has been destroyed
        /// </summary>
        /// <param name="msg"></param>
        [Server]
        public void ShipDestroyed(ShipDestroyMessage msg)
        {
            DestroyDespatch DD = new DestroyDespatch();
            DD.AggressorTeamID = msg.AggressorTeam;
            DD.AggressorID = msg.AggressorID;
            DD.SelfTeamID = msg.SelfTeam;
            DD.SelfID = msg.SelfID;
            DD.MiscID = msg.PlayerID;

            EventShipDestroyed(DD);
        }

        /// <summary>
        /// Sends across the server that
        /// a station has been destroyed
        /// </summary>
        /// <param name="msg"></param>
        [Server]
        public void StationDestroyed(StationDestroyMessage msg)
        {
            DestroyDespatch DD = new DestroyDespatch();
            DD.SelfID = msg.StationNetID;
            DD.MiscID = msg.StationSpawnID;
            DD.SelfTeamID = msg.StationTeamID;

            EventStationDestroyed(DD);
        }

        [Server]
        public void StationCreated(NetworkInstanceId Self)
        {
            CreateDispatch CD = new CreateDispatch();
            CD.PlayerID = -1;
            CD.Self = Self.Value;
            CD.TeamID = -1;
            EventStationCreated(CD);
        }

        #endregion
    }
}