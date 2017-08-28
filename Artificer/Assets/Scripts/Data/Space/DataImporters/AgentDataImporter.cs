using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System;

using Data.Space.Library;

namespace Data.Space.DataImporter
{

    /// <summary>
    /// Used by a centralized AI manager to import 
    /// agent data
    /// </summary>
    public class AgentDataImporter
    {
        #region PUBLIC INTERACTION

        /// <summary>
        /// Build the different agent types
        /// and returns them in an accessible format
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, AgentData> BuildAgents()
        {
            Dictionary<string, AgentData> returnVal 
                = new Dictionary<string, AgentData>();

            TextAsset txtAsset = (TextAsset)Resources.Load
               ("Space/Keys/ai_agent_key") as TextAsset;

            XmlDocument baseElement = new XmlDocument();
            baseElement.LoadXml(txtAsset.text);

            XmlNode elementContainer = baseElement.LastChild;

            foreach (XmlNode xmlElement
                    in elementContainer.ChildNodes)
            {
                AgentData agent = BuildAgent(xmlElement);
                returnVal.Add(agent.Name, agent);
            }

            return returnVal;
        }

        #endregion

        #region PRIVATE UTILITIES

        /// <summary>
        /// Creates an agent using the node and 
        /// returns the agent data
        /// </summary>
        /// <param name="agentNode"></param>
        /// <returns></returns>
        private static AgentData BuildAgent(XmlNode agentNode)
        {
            // Create and init agent
            AgentData newAgent = new AgentData()
            {
                Name = agentNode.Attributes["name"].Value,
                Type = agentNode.Attributes["type"].Value,
                Ship = new string[agentNode.ChildNodes.Count],
                Template = agentNode.Attributes["template"].Value,
                EngageDistance = agentNode.Attributes["engage"].Value,
                PursuitDistance = agentNode.Attributes["pursue"].Value,
                AttackDistance = agentNode.Attributes["attack"].Value,
                PullOffDistance = agentNode.Attributes["pulloff"].Value
            };

            // read ship names
            for (int i = 0; i < agentNode.ChildNodes.Count; i++)
                newAgent.Ship[i] = agentNode.ChildNodes[i].InnerText;

            return newAgent;
        }

        #endregion
    }
}
