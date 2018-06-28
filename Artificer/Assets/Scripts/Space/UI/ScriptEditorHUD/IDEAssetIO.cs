using Data.Space.Library;
using Data.UI;
using System;
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

        /// <summary>
        /// Recursive function that reads the inner data 
        /// of a node inter internal memory
        /// </summary>
        /// <param name="library"></param>
        /// <param name="subTag"></param>
        /// <param name="node"></param>
        /// <param name="groupID"></param>
        private void BuildSubTag(NodeLibrary library, XmlNode subTag,
            NodeData node, int groupID = -1)
        {
            switch (subTag.Name)
            {
                case "link":
                case "param":
                    node.AddIO(subTag, groupID);
                    break;
                case "type":
                    {
                        switch (subTag.InnerText)
                        {
                            case "number":
                                node.SupportedTypes.Add(NodeData.IO.IOType.NUM);
                                break;
                            case "string":
                                node.SupportedTypes.Add(NodeData.IO.IOType.STRING);
                                break;
                            case "bool":
                                node.SupportedTypes.Add(NodeData.IO.IOType.BOOL);
                                break;
                            case "vec2":
                                node.SupportedTypes.Add(NodeData.IO.IOType.VEC2);
                                break;
                            case "entity":
                                node.SupportedTypes.Add(NodeData.IO.IOType.ENTITY);
                                break;
                            case "alignment":
                                node.SupportedTypes.Add(NodeData.IO.IOType.ALIGNMENT);
                                break;
                            case "entityarray":
                                node.SupportedTypes.Add(NodeData.IO.IOType.ENTITYARRAY);
                                break;
                            case "numarray":
                                node.SupportedTypes.Add(NodeData.IO.IOType.NUMARRAY);
                                break;
                            case "stringarray":
                                node.SupportedTypes.Add(NodeData.IO.IOType.STRINGARRAY);
                                break;
                            case "vec2array":
                                node.SupportedTypes.Add(NodeData.IO.IOType.VEC2ARRAY);
                                break;
                            default:
                                node.SupportedTypes.Add(NodeData.IO.IOType.UNDEF);
                                break;
                        }
                        break;
                    }
                case "group":
                    {
                        foreach (XmlNode loopTag
                            in subTag.ChildNodes)
                        {
                            BuildSubTag(library, loopTag, node, 
                                Convert.ToInt32(subTag.Attributes["id"].Value));
                        }
                        break;
                    }
                case "description":
                    {
                        node.Description = subTag.InnerText;
                        break;
                    }
            }
        }
    }
}
