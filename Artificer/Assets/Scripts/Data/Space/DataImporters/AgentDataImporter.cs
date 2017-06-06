using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

namespace Data.Space.DataImporter
{

    /// <summary>
    /// Used by a centralized AI manager to import 
    /// agent data
    /// </summary>
    public class AgentDataImporter : MonoBehaviour
    {
        #region PUBLIC INTERACTION

        public static AgentData[] BuildTemplates()
        {
            TextAsset txtAsset = (TextAsset)Resources.Load
               ("Space/Keys/item_key") as TextAsset;

            XmlDocument baseElement = new XmlDocument();
            baseElement.LoadXml(txtAsset.text);

            XmlNode elementContainer = baseElement.LastChild;

            AgentData[] templates = new AgentData
                [elementContainer.ChildNodes.Count];

            int i = 0;

            foreach (XmlNode xmlElement
                    in elementContainer.ChildNodes)
            {
                templates[i++] = BuildAgent(xmlElement);
            }

            return templates;
        }

        #endregion

        /// <summary>
        /// Build an returns an AI template dependant on 
        /// the xml specifications
        /// </summary>
        /// <param name="agentNode"></param>
        /// <returns></returns>
        private static AgentData BuildAgent(XmlNode agentNode)
        {
            AgentData returnValue = new AgentData();

            returnValue.Type = agentNode.Attributes["name"].Value;

            returnValue.States = new
                AgentData.StateData[agentNode.ChildNodes.Count];

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
    }
}
