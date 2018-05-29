using Data.UI;
using System.Collections;
using System.Collections.Generic;
using UI.Effects;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Space.UI.IDE
{
    /// <summary>
    /// Used to determine the attribute types
    /// </summary>
    public enum NodeType { NUM, BOOL, OBJECT, ARRAY };

    public class NodePrefab : NodeViewer,
        IPointerEnterHandler, IPointerExitHandler,
        IDragHandler
    {
        #region EVENT

        public delegate void MouseEvent(NodePrefab Reference);

        public event MouseEvent OnMouseOver;

        public event MouseEvent OnMouseOut;

        public event MouseEvent OnMouseDown;

        public event MouseEvent OnMouseUp;

        public event MouseEvent OnDragNode;

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

        #endregion

        // Use this for initialization
        void Start()
        {

        }

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

                        m_mouseOver = false;
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

            DisplayItem(node);
        }

        #endregion

        #region IPOINTEREVENT

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (OnMouseOver != null)
                OnMouseOver(this);

            m_mouseOver = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {

        }

        public void OnDrag(PointerEventData eventData)
        {
            if (EditorManager.DraggedObj == null &&
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
