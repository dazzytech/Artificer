using Data.UI;
using System.Collections;
using System.Collections.Generic;
using UI;
using UI.Effects;
using UnityEngine;

namespace Space.UI.IDE
{
    /// <summary>
    /// Interacts with the scripting UI
    /// placing nodes and drawing links
    /// </summary>
    public class ScriptUI : MonoBehaviour
    {
        #region ATTIBUTES

        [SerializeField]
        private EditorManager m_con;

        [Header("UI Prefabs")]

        [SerializeField]
        private GameObject m_NodePrefab;

        [Header("UI Elements")]

        [SerializeField]
        private Transform m_scriptEditor;

        #endregion

        #region PUBLIC INTERACTION

        public void Initialize()
        {
            
        }

        public void UpdateUI()
        {
            if (EditorManager.SelectedObj != null)
            {
                if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                {
                    // Deselect object if we click outside selected piece or component window
                    if (!RectTransformExtension.InBounds
                        (EditorManager.SelectedObj.GetComponent<RectTransform>(),
                        Input.mousePosition))
                    {
                        EditorManager.SelectedObj = null;
                        return;
                    }
                }
            }
        }

        public NodePrefab PlaceNode(NodeData node)
        {
            GameObject nodeGO = Instantiate(m_NodePrefab);
            NodePrefab newNode = nodeGO.GetComponent<NodePrefab>();
            newNode.InitializeNode(node);
            nodeGO.transform.SetParent(m_scriptEditor);

            return newNode;
        }

        #endregion
    }
}