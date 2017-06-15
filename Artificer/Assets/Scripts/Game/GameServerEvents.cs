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

        public delegate void DestroyedEvent(DestroyDespatch DD);
        [SyncEvent]
        public static event DestroyedEvent EventShipDestroyed;
        [SyncEvent]
        public static event DestroyedEvent EventStationDestroyed;

        #endregion

        #region PUBLIC INTERACTION

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