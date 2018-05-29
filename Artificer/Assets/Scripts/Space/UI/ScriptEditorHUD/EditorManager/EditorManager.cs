using Data.Space.Library;
using Data.UI;
using System.Collections;
using System.Collections.Generic;
using UI.Effects;
using UnityEngine;

namespace Space.UI.IDE
{
    /// <summary>
    /// is the manager for both the editor and the 
    /// Node list and populates them with nodes
    /// Adds and removes nodes from the script viewer and saves and loads
    /// </summary>
    public class EditorManager : MonoBehaviour
    {
        public delegate void Create(NodeData node);

        #region ATTIBUTES

        [SerializeField]
        private EditorAttributes m_att;

        [HideInInspector]
        public static NodePrefab DraggedObj;

        [HideInInspector]
        public static NodePrefab HighlightedObj;

        [HideInInspector]
        public static NodePrefab SelectedObj;

        #endregion

        #region ACCESSOR

        /// <summary>
        /// Read only access for the available prefab
        /// </summary>
        public NodeLibrary Prefabs
        {
            get
            {
                return m_att.NodePrefabs;
            }
        }

        #endregion

        #region MONO BEHAVIOUR

        void Update()
        {
            if (m_att.AddedNodes.Count > 0)
            {
                m_att.ScriptHUD.UpdateUI();
                
                for (int i = 0; i < m_att.AddedNodes.Count; i++)
                {
                    NodePrefab node = m_att.AddedNodes[i];

                    if (DraggedObj == node && !node.Dragging)
                    {
                        if (!RectTransformExtension.InBounds
                            (m_att.ScriptHUD.GetComponent<RectTransform>(),
                            DraggedObj.transform.position))
                        {
                            DeleteNode(DraggedObj);
                            i--;
                        }

                        // now stop checking
                        DraggedObj = null;
                    }
                }
            }
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Load attributes and output to script and list
        /// </summary>
        public void Initialize(NodeLibrary nodePrefabs)
        {
            m_att.NodePrefabs = nodePrefabs;

            m_att.PrefabListHUD.GeneratePrefabs();

            m_att.ScriptHUD.Initialize();

            // generate a start node
            NodeData start = m_att.NodePrefabs
                [m_att.NodePrefabs.GetID("ENTRY")].Clone();

            m_att.AddedIDs.Add
                (start.InstanceID = 0);


            NodePrefab node = m_att.ScriptHUD.PlaceNode(start);
            node.transform.position = new Vector3(146, 425, 0);
            m_att.AddedNodes.Add(node);

            node.OnDragNode += StartDrag;
            node.OnMouseDown += MouseDown;
            node.OnMouseOver += MouseEnter;
            node.OnMouseOut += MouseLeave;

            m_att.EntryNode = node;
        }

        /// <summary>
        /// Clears and draws the new nodes to the script viewer
        /// </summary>
        /// <param name="agent"></param>
        public void LoadAgent(string agent)
        {

        }

        public void DeleteNode(NodePrefab node)
        {
            if(m_att.EntryNode == node)
                return;

            m_att.AddedIDs.Remove(node.Node.InstanceID);

            m_att.AddedNodes.Remove(node);

            GameObject.Destroy(node.gameObject);

            node.OnDragNode -= StartDrag;
            node.OnMouseDown -= MouseDown;
            node.OnMouseOver -= MouseEnter;
            node.OnMouseOut -= MouseLeave;
        }
        
        /// <summary>
        /// Creates an instance of 
        /// </summary>
        /// <param name="nID"></param>
        public void AddNode(NodeData prefab)
        {
            NodeData newNode = prefab.Clone();
            m_att.AddedIDs.Add
                (newNode.InstanceID = ReturnNextAvailableInstance());

            NodePrefab node = m_att.ScriptHUD.PlaceNode(newNode);

            node.OnDragNode += StartDrag;
            node.OnMouseDown += MouseDown;
            node.OnMouseOver += MouseEnter;
            node.OnMouseOut += MouseLeave;

            m_att.AddedNodes.Add(node);

            DraggedObj = node;
            DraggedObj.Dragging = true;
        }

        #endregion

        #region PRIVATE INTERACTION

        private int ReturnNextAvailableInstance()
        {
            int ID = 0;
            // Sort IDs numerically
            m_att.AddedIDs.Sort();
            // compare ID to each ID end result showed be the next available ID
            foreach (int otherID in m_att.AddedIDs)
            {
                if (otherID == ID)
                    ID = otherID + 1;
            }

            return ID;
        }

        public void StartDrag(NodePrefab node)
        {
            DraggedObj = SelectedObj = node;
        }

        public void MouseDown(NodePrefab node)
        {
            SelectedObj = node;
        }

        public void MouseEnter(NodePrefab node)
        {
            HighlightedObj = node;
        }

        public void MouseLeave(NodePrefab node)
        {
            if (HighlightedObj == node)
                HighlightedObj = null;
        }

        #endregion


    }
}