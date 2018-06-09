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
        private GameObject m_ioPrefab;

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

        #endregion

        #region REFERENCE

        [Header("References")]

        /// <summary>
        /// Node prefab created when clicked
        /// </summary>
        protected NodeData m_nodeData;

        protected Dictionary<NodeData.IO, IOPrefab> m_IOObjects
            = new Dictionary<NodeData.IO, IOPrefab>();

        #endregion

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Creates image and assigns delegate
        /// and component prefab
        /// </summary>
        /// <param name="GO"></param>
        /// <param name="create"></param>
        public virtual void DisplayItem(bool displayInput = false)
        {
            // Assign the data to our visual elements
            m_label.text = m_nodeData.Label;

            List<NodeData.IO> deleteList = new List<NodeData.IO>();

            foreach(NodeData.IO io in m_IOObjects.Keys)
            {
                if (!m_nodeData.Input.Contains(io)
                    && !m_nodeData.Output.Contains(io))
                    deleteList.Add(io);
            }

            foreach(NodeData.IO io in deleteList)
            {
                GameObject.Destroy(m_IOObjects[io].gameObject);
                m_IOObjects.Remove(io);
            }

            foreach (NodeData.IO input in m_nodeData.Input)
            {
                if (m_IOObjects.ContainsKey(input))
                    continue;

                GameObject inGO = Instantiate(m_ioPrefab);
                inGO.transform.SetParent(m_inputList);

                IOPrefab prefab = inGO.GetComponentInChildren<IOPrefab>();
                prefab.Initialize(input, true);

                if(displayInput)
                    prefab.UpdateInput();

                m_IOObjects.Add(input, prefab);
            }

            foreach (NodeData.IO output in m_nodeData.Output)
            {
                if (m_IOObjects.ContainsKey(output))
                    continue;

                GameObject outGO = Instantiate(m_ioPrefab);
                outGO.transform.SetParent(m_outputList);

                IOPrefab prefab = outGO.GetComponentInChildren<IOPrefab>();
                prefab.Initialize(output, false);

                m_IOObjects.Add(output, prefab);
            }
        }

        #endregion
    }
}
