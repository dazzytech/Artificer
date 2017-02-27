using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System.Collections;

using Space.GameFunctions;
using Networking;

namespace Space.UI.Spawn
{
    /// <summary>
    /// Interaction with spawn selector hud e.g.
    /// Select Ship, Select spawn, Spawn button down
    /// </summary>
    public class SpawnSelectorEventListener : MonoBehaviour
    {
        #region ATTRIBUTES

        private SpawnSelectorController _con;

        #endregion

        #region MONO BEHAVIOUR

        void Awake()
        {
            _con = GetComponent<SpawnSelectorController>();
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Sends our spawn info to the Game Controller
        /// todo: check we have ship and spawn selected
        /// </summary>
        public void SpawnPressed()
        {
            // For now we don't use spawn or ship info
            SpawnSelectionMessage ssm = new SpawnSelectionMessage();
            ssm.PlayerID = SystemManager.Space.ID;
            ssm.ShipID = 0;
            ssm.SpawnID = 0;

            SystemManager.singleton.client.Send((short)MSGCHANNEL.SPAWNPLAYER, ssm);
        }

        /// <summary>
        /// Set the controller countdown and enable spawn button
        /// </summary>
        /// <param name="delay"></param>
        public void SetSpawnTimer(float delay)
        {
            if (_con != null)
                _con.EnableSpawn(delay);
        }

        #endregion
    }
}
