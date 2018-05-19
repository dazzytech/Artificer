using UnityEngine;
using System.Collections;

namespace Data.Shared
{
    /// <summary>
    /// Data that links a ship with the user-made script
    /// </summary>
    public class NPCPrefabData
    {
        /// <summary>
        /// name of the ship that is spawned for the agent
        /// </summary>
        public string Ship;

        /// <summary>
        /// The name of the script that is generated with the agent
        /// </summary>
        public string Agent;
    }
}