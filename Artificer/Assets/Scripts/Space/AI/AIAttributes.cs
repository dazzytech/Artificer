using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
using UnityEngine.Networking;
using Data.Space;

namespace Space.AI
{ 
    /// <summary>
    /// Stores all attributes pertaining to ai systems
    /// e.g. ai ships etc for use by ai manager
    /// </summary>
    public class AIAttributes : MonoBehaviour
    {
        /// <summary>
        /// A reference to an agent template used
        /// for spawning an AI agent
        /// </summary>
        public Dictionary<string, AgentData> AgentLibrary;
    }
}
