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

        [HideInInspector]
        public static IOPrefab IODraggingLink;

        [HideInInspector]
        public static IOPrefab IOOverLink;

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

        public List<LinkRender> RenderList
        {
            get
            {
                return m_att.RenderList;
            }
        }

        public NodeData ScriptEntry
        {
            get
            {
                return m_att.EntryNode.Node;
            }
        }

        #endregion

        #region MONO BEHAVIOUR

        void Update()
        {
            if (m_att.AddedNodes.Count > 0)
            {
                m_att.ScriptHUD.UpdateUI();

                NodePrefab delete = null;
                
                foreach (NodePrefab node in m_att.AddedNodes.Values)
                {
                    if (node == null)
                        continue;

                    if (DraggedObj == node && !node.Dragging)
                    {
                        if (!RectTransformExtension.InBounds
                            (m_att.ScriptHUD.GetComponent<RectTransform>(),
                            DraggedObj.transform.position))
                            delete = node;

                        // now stop checking
                        DraggedObj = null;
                        
                        break;
                    }
                }

                DeleteNode(delete);
            }
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Load attributes and output to script and list
        /// </summary>
        public void Initialize(NodeLibrary nodePrefabs)
        {
            // Create and display a log that displays each node object loaded into the attributes
            Debug.Log("Node Prefabs Successfully Loaded:");
            foreach (NodeData prefab in nodePrefabs)
            {
                Debug.Log(string.Format("Node: {0} - {1}", prefab.Label, prefab.Description));
            }

            m_att.NodePrefabs = nodePrefabs;

            m_att.PrefabListHUD.GeneratePrefabs();

            // generate a start node
            NodeData start = m_att.NodePrefabs
                [m_att.NodePrefabs.GetID("ENTRY")].Clone();

            m_att.AddedIDs.Add
                (start.InstanceID = 0);


            NodePrefab node = m_att.ScriptHUD.PlaceNode(start);
            node.transform.position = new Vector3(146, 425, 0);
            m_att.AddedNodes.Add(0, node);

            AddListeners(node);

            m_att.EntryNode = node;
        }

        /// <summary>
        /// Clears and draws the new nodes to the script viewer
        /// </summary>
        /// <param name="agent"></param>
        public void LoadAgent(string agent)
        {

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

            AddListeners(node);

            m_att.AddedNodes.Add(newNode.InstanceID, node);

            DraggedObj = node;
            DraggedObj.Dragging = true;
        }

        #endregion

        #region PRIVATE INTERACTION

        #region EDIT LISTENERS

        /// <summary>
        /// Assign the node events to the listeners
        /// </summary>
        /// <param name="node"></param>
        private void AddListeners(NodePrefab node)
        {
            node.OnDragNode += NodeDrag;
            node.OnDragIO += IODrag;
            node.OnMouseDown += MouseDown;
            node.OnDownIO += IODown;
            node.OnEnterIO += IOEnter;
            node.OnExitIO += IOLeave;
            node.OnMouseOver += MouseEnter;
            node.OnMouseOut += MouseLeave;
            node.OnMouseUp += MouseUp;
        }

        /// <summary>
        /// remove all listeners 
        /// </summary>
        /// <param name="node"></param>
        private void RemoveListeners(NodePrefab node)
        {
            node.OnDragNode -= NodeDrag;
            node.OnDragIO -= IODrag;
            node.OnMouseDown -= MouseDown;
            node.OnEnterIO -= IOEnter;
            node.OnExitIO -= IOLeave;
            node.OnMouseOver -= MouseEnter;
            node.OnMouseOut -= MouseLeave;
            node.OnMouseUp -= MouseUp;
        }

        #endregion

        private void DeleteNode(NodePrefab node)
        {
            if (m_att.EntryNode == node || node == null)
                return;
            if (m_att.AddedNodes.ContainsValue(node))
            {
                m_att.AddedIDs.Remove(node.Node.InstanceID);

                RemoveListeners(node);

                m_att.AddedNodes.Remove(node.Node.InstanceID);

                foreach (IOPrefab io in node.IOList.Values)
                    UnlinkIO(io, node);

                GameObject.Destroy(node.gameObject);
            }
        }

        private void UnlinkIO
            (IOPrefab io, NodePrefab node)
        {
            NodeData.IO otherIO = io.IO.LinkedIO;

            if (otherIO == null)
                return;

            NodeData other = otherIO.Node;

            IOPrefab otherprefab = m_att.AddedNodes[other.InstanceID].GetIO(otherIO);
            other.DereferenceNode(otherIO);
            m_att.AddedNodes[other.InstanceID].DisplayItem(true);
            otherprefab.UpdateNode();
            m_att.AddedNodes[other.InstanceID].ResetType();
            node.ResetType();

            node.Node.DereferenceNode(io.IO);
            node.DisplayItem(true);
            io.UpdateNode();

            m_att.RenderList.Remove(m_att.RenderList.Find
                (x => (x.Start == io) || (x.End == io)
                && (x.Start == otherprefab) || (x.End == otherprefab)));
        }

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

        #region LISTENERS

        public void NodeDrag(NodePrefab node)
        {
            DraggedObj = SelectedObj = node;
        }

        public void IODrag(IOPrefab io, NodePrefab node)
        {
            IODraggingLink = io;
            IODraggingLink.Close();
            SelectedObj = node;
        }

        public void MouseDown(NodePrefab node)
        {
            if(IODraggingLink == null)
                SelectedObj = node;
        }

        public void MouseEnter(NodePrefab node)
        {
            HighlightedObj = node;
        }

        public void IOEnter(IOPrefab io, NodePrefab node)
        {
            if (IODraggingLink != null && node != SelectedObj)
            {
                // also make sure ios are not both input or output
                if (IODraggingLink.In == io.In)
                    return;

                // check here that the io types are matching, otherwise quit
                if (IODraggingLink.IO.Type != io.IO.Type)
                {
                    // detect if the type of either is unassigned
                    if (io.IO.Type == NodeData.IO.IOType.UNDEF
                        || io.IO.Type == NodeData.IO.IOType.UNDEFSINGLE)
                    {
                        if (io.IO.Node.SupportedTypes.Contains(IODraggingLink.IO.CurrentType))
                        {
                            // assign the io node to the sub var type
                            node.SetType(IODraggingLink.IO.CurrentType);
                        }
                        else
                            return;
                    }
                    else if (IODraggingLink.IO.Type == NodeData.IO.IOType.UNDEF
                        || io.IO.Type == NodeData.IO.IOType.UNDEFSINGLE)
                    {
                        if (IODraggingLink.IO.Node.SupportedTypes.Contains(io.IO.CurrentType))
                        {
                            // assign the io node to the sub var type
                            SelectedObj.SetType(io.IO.CurrentType);
                        }
                        else
                            return;
                    }
                    else
                        return;
                }
                else if (io.IO.Type == NodeData.IO.IOType.UNDEF 
                    || io.IO.Type == NodeData.IO.IOType.UNDEFSINGLE)
                    return;

                io.Close();
                IOOverLink = io;
            }
            
        }

        public void IODown(IOPrefab io, NodePrefab node)
        {
            NodeData.IO otherIO = io.IO.LinkedIO;

            if (otherIO == null)
                return;

            UnlinkIO(io, node);
        }

        public void IOLeave(IOPrefab io, NodePrefab node)
        {
            if (IOOverLink == io)
            {
                node.ResetType();
                SelectedObj.ResetType();
                IOOverLink.Open();
                IOOverLink = null;
            }
        }

        public void MouseLeave(NodePrefab node)
        {
            if (HighlightedObj == node)
                HighlightedObj = null;
        }

        public void MouseUp(NodePrefab node) 
        {
            if(IODraggingLink != null)
            {
                IODraggingLink.UpdateNode();

                // if we are dragging a link
                // check if the is a node the mouse is currently over
                // if so then create the link
                if(IOOverLink != null)
                {
                    if (IOOverLink != IODraggingLink &&
                        SelectedObj != HighlightedObj)
                    {
                        if(IODraggingLink.IO.LinkedIO != null)
                            UnlinkIO(IODraggingLink, SelectedObj);

                        SelectedObj.Node.AssignToNode(IODraggingLink.IO, IOOverLink.IO);
                        SelectedObj.DisplayItem(true);
                        IODraggingLink.UpdateNode();
                        IODraggingLink.UpdateInput();

                        HighlightedObj.Node.AssignToNode(IOOverLink.IO, IODraggingLink.IO);
                        HighlightedObj.DisplayItem(true);
                        IOOverLink.UpdateNode();
                        IOOverLink.UpdateInput();

                        LinkRender link = new LinkRender();
                        link.Start = IODraggingLink;
                        link.End = IOOverLink;
                        link.Colour = IODraggingLink.Colour;

                        m_att.RenderList.Add(link);
                    }
                }
            }

            IODraggingLink = null;
            IOOverLink = null;
        }

        #endregion

        #endregion

        #region COROUTINE



        #endregion
    }
}