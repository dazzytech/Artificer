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
        /// Builds the list of templates that
        /// the ai agents 
        /// </summary>
        /// <param name="library"></param>
        public static void BuildTemplates(AITemplateLibrary library)
        {
            TextAsset txtAsset = (TextAsset)Resources.Load
               ("Space/Keys/ai_template_key") as TextAsset;

            XmlDocument baseElement = new XmlDocument();
            baseElement.LoadXml(txtAsset.text);

            XmlNode elementContainer = baseElement.LastChild;

            foreach (XmlNode xmlElement
                    in elementContainer.ChildNodes)
            {
                library.Add(BuildTemplate(xmlElement));
            }
        }

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
        /// Build an returns an AI template dependant on 
        /// the xml specifications
        /// </summary>
        /// <param name="agentNode"></param>
        /// <returns></returns>
        private static AITemplateData BuildTemplate(XmlNode agentNode)
        {
            AITemplateData returnValue = new AITemplateData();

            returnValue.Type = agentNode.Attributes["name"].Value;

            returnValue.States = new
                AITemplateData.StateData[agentNode.ChildNodes.Count];

            int i = 0;

            foreach (XmlNode xmlElement
                    in agentNode.ChildNodes)
            {
                returnValue.States[i].type = xmlElement.Attributes["type"].Value;

                returnValue.States[i].TransIDs = new int
                    [xmlElement.ChildNodes.Count];

                returnValue.States[i].StateIDs = new int
                    [xmlElement.ChildNodes.Count];

                int a = 0;

                foreach (XmlNode stateElement
                    in xmlElement.ChildNodes)
                {

                    returnValue.States[i].TransIDs[a] = System.Convert.ToInt32
                    (stateElement.Attributes["transID"].Value);

                    returnValue.States[i].StateIDs[a++] = System.Convert.ToInt32
                        (stateElement.Attributes["stateID"].Value);
                }

                i++;
            }

            return returnValue;
        }

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
