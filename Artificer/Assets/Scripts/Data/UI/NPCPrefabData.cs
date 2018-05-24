using UnityEngine;
using System.Collections;

namespace Data.UI
{
    /// <summary>
    /// Data that links a ship with the user-made script
    /// </summary>
    [System.Serializable]
    public class NPCPrefabData
    { 
        /// <summary>
        /// name of the ship that is spawned for the agent
        /// </summary>
        public int Ship;

        /// <summary>
        /// The name of the script that is generated with the agent
        /// </summary>
        public string Agent;
    }
}