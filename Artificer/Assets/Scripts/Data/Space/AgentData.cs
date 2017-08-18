using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data.Space
{
    /// <summary>
    /// Contains the information for each
    /// individual agents e.g. raiders, miners
    /// </summary>
    public class AgentData
    {
        // The name of the ship "raider small"
        public string Name;

        // The type of the ship e.g. light scout
        public string Type;

        // The template for the agent behaviour
        public string Template;

        // Ship that the agent uses
        public string[] Ship;

        #region FSM VARIABLES

        /// <summary>
        /// How close we get to the target before moving to engage/pursue
        /// </summary>
        public string EngageDistance;

        /// <summary>
        /// Distance before agent breaks off
        /// </summary>
        public string PursuitDistance;

        /// <summary>
        /// How close the agent needs to be for the attack state
        /// </summary>
        public string AttackDistance;

        /// <summary>
        /// How close a ship can get before the ship pulls off
        /// </summary>
        public string PullOffDistance;

        #endregion
    }
}
