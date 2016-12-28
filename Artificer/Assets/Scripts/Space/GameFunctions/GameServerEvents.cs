using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

using Networking;

namespace Space.GameFunctions
{
    /// <summary>
    /// dispatches events that are to trigger 
    /// other server listeners
    /// </summary>
    public class GameServerEvents : NetworkBehaviour
    {
        #region EVENTS

        public delegate void DestroyedEvent(DestroyDespatch DD);
        public static event DestroyedEvent OnShipDestroyed;
        public static event DestroyedEvent OnStationDestroyed;

        #endregion

        #region PUBLIC INTERACTION

        public void ShipDestroyed(ShipDestroyMessage msg)
        {
            DestroyDespatch DD = new DestroyDespatch();
            DD.AggressorTag = msg.AggressorTag;
            DD.AlignmentLabel = msg.AlignmentLabel;
            DD.Self = msg.SelfID;

            OnShipDestroyed(DD);
        }

        public void StationDestroyed(StationDestroyMessage msg)
        {
            DestroyDespatch DD = new DestroyDespatch();
            DD.Self = msg.SelfID;
            DD.MiscID = msg.ID;

            OnStationDestroyed(DD);
        }
        #endregion
    }
}