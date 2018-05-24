using Data.Space.Library;
using Data.UI;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Space.UI.IDE
{
    /// <summary>
    /// Loads data from xml and updates attributes
    /// </summary>
    public class IDEAssetIO : MonoBehaviour
    {
        [SerializeField]
        private IDEAttributes m_att;

        public NodeLibrary LoadPrefabData()
        {
            NodeLibrary library = new NodeLibrary();

            TextAsset txtAsset = (TextAsset)Resources.Load 
                ("IDE/prefabs") as TextAsset;

            XmlDocument baseElement = new XmlDocument();
            baseElement.LoadXml(txtAsset.text);

            XmlNode elementContainer = baseElement.LastChild;

            foreach (XmlNode xmlElement
                    in elementContainer.ChildNodes)
            {
                BuildCategory(library, xmlElement, xmlElement.Attributes["label"].Value);
            }
            return library;
        }

        private void BuildCategory
            (NodeLibrary library, XmlNode elementContainer, string category)
        {
            library.Categories.Add(category);

            foreach (XmlNode xmlElement
                    in elementContainer.ChildNodes)
            {
                NodeData node = new NodeData();

                // populate the node attributes
                node.Label = xmlElement.Attributes["label"].Value;

                node.Category = category;

                // populate the data with subtags
                foreach (XmlNode subTag
                        in xmlElement.ChildNodes)
                {
                    BuildSubTag(library, subTag, node);
                }

                library.Add(node);
            }

        }

        private void BuildSubTag(NodeLibrary library, XmlNode subTag, NodeData node)
        {
            switch (subTag.Name)
            {
                case "link":
                    {
                        NodeData.IO io = new NodeData.IO();
                        io.Type = NodeData.IO.IOType.LINK;
                        io.ID = subTag.Attributes["id"].Value;
                        io.NodeID = "-1";
                        if (subTag.Attributes["in"].Value == "true")
                            node.Input.Add(io);
                        else
                            node.Output.Add(io);
                        break;
                    }
                case "param":
                    {
                        NodeData.IO io = new NodeData.IO();
                        io.Type = NodeData.IO.IOType.PARAM;
                        io.ID = subTag.Attributes["id"].Value;
                        io.NodeID = node.ID.ToString();
                        break;
                    }
                case "type":
                    {
                        node.SupportedTypes.Add(subTag.InnerText);
                        break;
                    }
                case "loop":
                    {
                        foreach (XmlNode loopTag
                            in subTag.ChildNodes)
                        {
                            BuildSubTag(library, loopTag, node);
                        }
                        break;
                    }
                case "script":
                    {
                        node.Script = string.Concat(node.Script, subTag.InnerText);
                        break;
                    }
            }
        }
    }
}
