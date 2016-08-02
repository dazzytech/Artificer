using UnityEngine;
using System.Collections;

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
            GameManager.GameMSG.CmdSpawnPlayerAt(GameManager.Space.ID, 0, 0);
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
