
using Data.UI;
using System.Collections;
using System.Collections.Generic;
using UI.Effects;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Space.UI.IDE
{
    public class NodePrefab : NodeViewer,
        IPointerEnterHandler, IPointerExitHandler,
        IDragHandler
    {
        #region EVENT

        public delegate void NodeEvent(NodePrefab Reference);

        public delegate void IOEvent(IOPrefab Reference, NodePrefab NodeReference);

        public event NodeEvent OnMouseOver;

        public event NodeEvent OnMouseOut;

        public event NodeEvent OnMouseDown;

        public event NodeEvent OnMouseUp;

        public event NodeEvent OnDragNode;

        public event IOEvent OnDragIO;

        public event IOEvent OnDownIO;

        public event IOEvent OnEnterIO;

        public event IOEvent OnExitIO;

        #endregion

        #region ATTRIBUTES

        #region HUD ELEMENTS

        /// <summary>
        /// Image that appears 
        /// on the component item
        /// </summary>
        [SerializeField]
        private RawImage m_nodeImage;

        #endregion

        /// <summary>
        /// detects if selected but not 
        /// dragged
        /// </summary>
        private bool m_selected = true;

        /// <summary>
        /// If our component is currently
        /// being dragged
        /// </summary>
        private bool m_isDragging = false;

        /// <summary>
        /// Detect if the component is over 
        /// our component
        /// </summary>
        private bool m_mouseOver = false;

        #endregion

        #region ACCESSOR

        public bool Dragging
        {
            get { return m_isDragging; }
            set { m_isDragging = value; }
        }

        /// <summary>
        /// The data that this prefab represents
        /// </summary>
        public NodeData Node
        {
            get { return m_nodeData; }
        }

        public Dictionary<NodeData.IO, IOPrefab> IOList
        {
            get { return m_IOObjects; }
        }

        #endregion

        private void Update()
        {
            if (m_isDragging)
            {
                if (!Input.GetMouseButton(0))
                {
                    if (OnMouseUp != null)
                        OnMouseUp(this);

                    m_isDragging = false;

                    return;
                }

                if (EditorManager.DraggedObj == this)
                    transform.position = Input.mousePosition;
            }
            else if (m_selected)
            {
                if (EditorManager.SelectedObj != this)
                    m_selected = false;
            }
            else
            {
                // Proceed with selection testing
                if (RectTransformExtension.InBounds
                   (m_nodeImage.rectTransform, Input.mousePosition))
                {
                    if (m_mouseOver && EditorManager.HighlightedObj == this)
                    {
                        if (Input.GetMouseButton(0) && EditorManager.DraggedObj == null)
                        {
                            if (OnMouseDown != null)
                                OnMouseDown(this);

                            m_selected = true;
                        }
                    }
                }
                else
                {
                    if (m_mouseOver)
                    {
                        if (OnMouseOut != null)
                            OnMouseOut(this);

                        foreach (IOPrefab io in m_IOObjects.Values)
                        {
                            if (OnExitIO != null)
                                OnExitIO(io, this);
                        }

                        m_mouseOver = false;
                    }
                }
            }

            if (Input.GetMouseButton(1))
            {
                foreach (IOPrefab io in m_IOObjects.Values)
                {
                    if (RectTransformExtension.InBounds(io.IconBounds, Input.mousePosition))
                    {
                        OnDownIO(io, this);
                    }
                }
            }
        }

        #region PUBLIC INTERACTION

        /// <summary>
        /// Builds the NodeOutput and save node data
        /// </summary>
        /// <param name="node"></param>
        public void InitializeNode(NodeData node)
        {
            m_nodeData = node;

            DisplayItem(true);
        }

        /// <summary>
        /// Will retrieve the relevent IO prefab using
        /// the IO data index
        /// </summary>
        /// <param name="io"></param>
        /// <returns></returns>
        public IOPrefab GetIO(NodeData.IO io)
        {
            return m_IOObjects[io];
        }

        public void SetType(NodeData.IO.IOType type)
        {
            foreach(IOPrefab io in m_IOObjects.Values)
            {
                if (io.IO.CurrentType == NodeData.IO.IOType.UNDEF)
                    io.IO.TempVar = type;
                else if (io.IO.CurrentType == NodeData.IO.IOType.UNDEFSINGLE)
                    io.IO.TempVar = IOGetSingle(type);

                io.UpdateNode();
                io.UpdateInput();
            }
        }

        public void ResetType()
        {
            foreach (IOPrefab io in m_IOObjects.Values)
            {
                io.IO.TempVar = NodeData.IO.IOType.UNDEF;

                io.UpdateNode();
                io.UpdateInput();
            }
        }

        #endregion

        /// <summary>
        /// When an input is an array
        /// this will return the stored var type in the array
        /// </summary>
        public NodeData.IO.IOType IOGetSingle(NodeData.IO.IOType CurrentType)
        {
            switch (CurrentType)
            {
                case NodeData.IO.IOType.NUMARRAY:
                    return NodeData.IO.IOType.NUM;
                case NodeData.IO.IOType.STRINGARRAY:
                    return NodeData.IO.IOType.STRING;
                case NodeData.IO.IOType.ENTITYARRAY:
                    return NodeData.IO.IOType.ENTITY;
                case NodeData.IO.IOType.VEC2ARRAY:
                    return NodeData.IO.IOType.VEC2;
                default:
                    return CurrentType;
            }
        }

        #region IPOINTEREVENT

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (OnMouseOver != null)
                OnMouseOver(this);

            // Detect if the mouse is over a IO icon, we will want to start dragging that instead
            foreach (IOPrefab io in m_IOObjects.Values)
            {
                if (RectTransformExtension.InBounds(io.IconBounds, Input.mousePosition))
                {
                    if (OnEnterIO != null)
                        OnEnterIO(io, this);
                }
            }

            m_mouseOver = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {

        }

        public void OnDrag(PointerEventData eventData)
        {
            if (EditorManager.IODraggingLink == null && !m_isDragging)
            {
                // Detect if the mouse is over a IO icon, we will want to start dragging that instead
                foreach (IOPrefab io in m_IOObjects.Values)
                {
                    if (RectTransformExtension.InBounds(io.IconBounds, Input.mousePosition))
                    {
                        if (OnDragIO != null)
                            OnDragIO(io, this);

                        m_isDragging = true;
                        // currently over an icon
                        return;
                    }
                }
            }

            if (EditorManager.DraggedObj == null && !m_isDragging &&
                eventData.button == PointerEventData.InputButton.Left)
            {
                if (OnDragNode != null)
                    OnDragNode(this);

                m_isDragging = true;
            }
        }

        #endregion
    }
}
