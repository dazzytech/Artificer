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
    }
}
