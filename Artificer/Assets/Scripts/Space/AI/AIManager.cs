using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Data.Space.DataImporter;
using Data.Space;
using Space.SpawnManager;

namespace Space.AI
{
    /// <summary>
    /// Centralized system for managing ai states
    /// </summary>
    public class AIManager : NetworkBehaviour
    {
        #region ATTRIBUTES

        [SerializeField]
        private AIAttributes m_att;

        #region SPAWN MANAGERS

        [SerializeField]
        private RaiderSpawnManager m_raiderSpawn;

        #endregion

        #endregion

        #region ACCESSORS

        /// <summary>
        /// Getter for agent ships for spawner objects
        /// </summary>
        public Dictionary<string, AgentData> AgentLibrary
        {
            get
            {
                return m_att.AgentLibrary;
            }
        }

        #endregion

        #region MONO BEHAVIOUR

        /// <summary>
        /// Begin heatbeats and 
        /// object seeking
        /// </summary>
        void Start()
        {
            // Begin the process of importing agent data
            m_att.AgentLibrary = AgentDataImporter.BuildAgents();       
        }

        #endregion

        #region COROUTINE

        #endregion
    }
}
