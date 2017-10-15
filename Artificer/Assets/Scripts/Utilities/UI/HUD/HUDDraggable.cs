using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UI.Effects;

namespace UI
{
    /// <summary>
    /// Attached to UI image elements that can be
    /// dragged
    /// </summary>
    class HUDDraggable: MonoBehaviour, 
        IPointerEnterHandler, 
        IPointerExitHandler,
        IDragHandler
    {
        #region EVENT

        /// <summary>
        /// Listens for events called when
        /// player mouse interacts with element
        /// </summary>
        /// <param name="Reference"></param>
        public delegate void MouseEvent(HUDDraggable Reference);

        public event MouseEvent OnMouseOver;

        public event MouseEvent OnMouseOut;

        public event MouseEvent OnMouseDown;

        public event MouseEvent OnMouseUp;

        public event MouseEvent OnDragComponent;

        #endregion

        #region ATTRIBUTES

        #region HUD ELEMENTS

        /// <summary>
        /// Image that appears 
        /// on the component item
        /// </summary>
        [SerializeField]
        private RawImage m_componentImage;

        #endregion

        /// <summary>
        /// If our HUD Element is currently
        /// being dragged
        /// </summary>
        private bool m_isDragging = false;

        /// <summary>
        /// Detect if the mouse is inside our border
        /// </summary>
        private bool m_mouseOver = false;

        #endregion

        #region ACCESSORS

        public bool Dragging
        {
            get { return m_isDragging; }
            set { m_isDragging = value; }
        }

        #endregion

        #region MONOBEHAVIOUR

        protected virtual void Update()
        {
            if (Dragging)
            {
                if (!Input.GetMouseButton(0))
                {
                    if (OnMouseUp != null)
                        OnMouseUp(this);

                    m_isDragging = false;

                    // Change our colour to a highlight
                    m_componentImage.color = new Color(.75f, .75f, .2f, .4f);

                    return;
                }

                transform.position = Input.mousePosition;
            }
            else
            {
                // Proceed with selection testing
                if (RectTransformExtension.InBounds
                   (GetComponent<RectTransform>(), Input.mousePosition))
                {
                    if (m_mouseOver)
                    {
                        if (Input.GetMouseButton(0))
                        {
                            if (OnMouseDown != null)
                                OnMouseDown(this);
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

                        m_componentImage.color = new Color(1, 1, 1, 1);
                    }
                }
            }
        }

        #endregion

        #region IPOINTEREVENT

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (OnMouseOver != null)
                OnMouseOver(this);

            m_mouseOver = true;

                m_componentImage.color = new Color(.75f, .75f, .2f, .4f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {

        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (OnDragComponent != null)
                    OnDragComponent(this);

                m_isDragging = true;
            }
        }

        #endregion
    }
}
