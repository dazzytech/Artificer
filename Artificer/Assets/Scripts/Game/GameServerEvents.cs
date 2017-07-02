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
        public void ShipCreated(NetworkInstanceId Self, int ID)
        {
            CreateDispatch CD = new CreateDispatch();
            CD.PlayerID = ID;
            CD.Self = Self.Value;
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
            DD.AggressorTag = msg.AggressorTag;
            DD.AlignmentLabel = msg.AlignmentLabel;
            DD.Self = msg.SelfID;
            DD.MiscID = msg.ID;

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
            DD.Self = msg.SelfID;
            DD.MiscID = msg.ID;

            EventStationDestroyed(DD);
        }

        #endregion
    }
}