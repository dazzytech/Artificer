using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

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

        public static void BuildTemplates(AITemplateLibrary library)
        {
            TextAsset txtAsset = (TextAsset)Resources.Load
               ("Space/Keys/ai_template_key") as TextAsset;

            XmlDocument baseElement = new XmlDocument();
            baseElement.LoadXml(txtAsset.text);

            XmlNode elementContainer = baseElement.LastChild;

            int i = 0;

            foreach (XmlNode xmlElement
                    in elementContainer.ChildNodes)
            {
                library.Add(BuildTemplate(xmlElement));
            }
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

        #endregion
    }
}
