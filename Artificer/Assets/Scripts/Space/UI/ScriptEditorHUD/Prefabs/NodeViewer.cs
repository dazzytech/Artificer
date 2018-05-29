using Data.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Space.UI.IDE
{
    public class NodeViewer : MonoBehaviour
    {
        #region ATTRIBUTES

        #region PREFABS

        [Header("UI Prefabs")]

        [SerializeField]
        private GameObject m_inputPrefab;

        [SerializeField]
        private GameObject m_outputPrefab;

        #endregion

        #region UI ELEMENTS

        [Header("UI Elements")]

        /// <summary>
        /// As the node viewer is made up of multiple pieces
        /// instead of using a single image as the boundary, use the 
        /// couldary of the entire prefab
        /// </summary>
        [SerializeField]
        private RectTransform m_bounds;

        /// <summary>
        /// the list of links and params that connect into the node
        /// </summary>
        [SerializeField]
        private Transform m_inputList;

        /// <summary>
        /// List of links and params that connect to other nodes
        /// </summary>
        [SerializeField]
        private Transform m_outputList;

        /// <summary>
        /// The name of the component (prefab name)
        /// </summary>
        [SerializeField]
        private Text m_label;

        #region ICONS

        [Header("Icon Images")]

        [SerializeField]
        private Texture2D m_undef;

        [SerializeField]
        private Texture2D m_num;

        [SerializeField]
        private Texture2D m_bool;

        [SerializeField]
        private Texture2D m_object;

        [SerializeField]
        private Texture2D m_objArray;

        [SerializeField]
        private Texture2D m_exec;

        #endregion

        #endregion

        #region REFERENCE

        [Header("References")]

        /// <summary>
        /// Node prefab created when clicked
        /// </summary>
        protected NodeData m_nodeData;

        #endregion

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Creates image and assigns delegate
        /// and component prefab
        /// </summary>
        /// <param name="GO"></param>
        /// <param name="create"></param>
        public virtual void DisplayItem(NodeData node)
        {
            m_nodeData = node;

            // Assign the data to our visual elements
            m_label.text = node.Label;

            foreach (NodeData.IO input in node.Input)
            {
                GameObject inPrefab = Instantiate(m_inputPrefab);
                inPrefab.transform.SetParent(m_inputList);

                inPrefab.GetComponentInChildren<Text>().text = input.Label;

                if (input.Type == NodeData.IO.IOType.LINK)
                    inPrefab.GetComponentInChildren<RawImage>().texture = m_exec;
                else
                {
                    // assign icon based on type
                    switch (input.Var)
                    {
                        case NodeData.IO.VarType.UNDEF:
                            inPrefab.GetComponentInChildren<RawImage>().texture = m_undef;
                            break;
                        case NodeData.IO.VarType.NUM:
                            inPrefab.GetComponentInChildren<RawImage>().texture = m_num;
                            break;
                        case NodeData.IO.VarType.BOOL:
                            inPrefab.GetComponentInChildren<RawImage>().texture = m_bool;
                            break;
                        case NodeData.IO.VarType.OBJECT:
                            inPrefab.GetComponentInChildren<RawImage>().texture = m_object;
                            break;
                        case NodeData.IO.VarType.ARRAY:
                            inPrefab.GetComponentInChildren<RawImage>().texture = m_objArray;
                            break;
                    }
                }
            }

            foreach (NodeData.IO output in node.Output)
            {
                GameObject outPrefab = Instantiate(m_outputPrefab);
                outPrefab.transform.SetParent(m_outputList);

                outPrefab.GetComponentInChildren<Text>().text = output.Label;

                if (output.Type == NodeData.IO.IOType.LINK)
                    outPrefab.GetComponentInChildren<RawImage>().texture = m_exec;
                else
                {
                    // assign icon based on type
                    switch (output.Var)
                    {
                        case NodeData.IO.VarType.UNDEF:
                            outPrefab.GetComponentInChildren<RawImage>().texture = m_undef;
                            break;
                        case NodeData.IO.VarType.NUM:
                            outPrefab.GetComponentInChildren<RawImage>().texture = m_num;
                            break;
                        case NodeData.IO.VarType.BOOL:
                            outPrefab.GetComponentInChildren<RawImage>().texture = m_bool;
                            break;
                        case NodeData.IO.VarType.OBJECT:
                            outPrefab.GetComponentInChildren<RawImage>().texture = m_object;
                            break;
                        case NodeData.IO.VarType.ARRAY:
                            outPrefab.GetComponentInChildren<RawImage>().texture = m_objArray;
                            break;
                    }
                }
            }
        }

        #endregion
    }
}
