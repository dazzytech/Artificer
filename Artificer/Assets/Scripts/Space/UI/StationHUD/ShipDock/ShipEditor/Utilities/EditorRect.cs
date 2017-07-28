using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace Space.UI.Station.Utility
{
    /// <summary>
    /// Manages the interaction with the editor
    /// rect e.g. zoom, scroll
    /// </summary>
    public class EditorRect : MonoBehaviour, 
        IPointerDownHandler, IBeginDragHandler,
        IEndDragHandler, IDragHandler
    {
        #region ATTRIBUTES

        #region ZOOM

        /// <summary>
        /// Level of magnifcation
        /// </summary>
        private float m_zoomLevel;

        /// <summary>
        /// starting scale
        /// </summary>
        private Vector3 m_originalScale;

        #endregion

        #region POSITION

        /// <summary>
        /// Distance to move the trasnform
        /// </summary>
        private Vector2 m_dragDelta = new Vector2();

        /// <summary>
        /// Previous position
        /// </summary>
        private Vector2 m_prevMPos = new Vector2();

        private Vector3 m_originalPos;

        /// <summary>
        /// If we are tracking the mouse pos
        /// </summary>
        private bool m_tracking = false;

        #endregion

        bool DblClick = false;

        #endregion

        #region MONO BEHAVIOUR

        void Start()
        {
            m_zoomLevel = 1;
            m_originalScale = transform.localScale;
        }

        void Update()
        {
            if (Input.mouseScrollDelta.y != 0)
            {
                if (Input.mouseScrollDelta.y > 0)
                    ZoomIn();
                else
                    ZoomOut();
            }

            // Detect keypresses
            if (Input.GetKey(Control_Config.GetKey("dragUp", "editor")))
                m_dragDelta = new Vector2(0, -5f);
            if (Input.GetKey(Control_Config.GetKey("dragDown", "editor")))
                m_dragDelta = new Vector2(0, 5f);
            if (Input.GetKey(Control_Config.GetKey("dragLeft", "editor")))
                m_dragDelta = new Vector2(5f, 0);
            if (Input.GetKey(Control_Config.GetKey("dragRight", "editor")))
                m_dragDelta = new Vector2(-5f, 0);

            if (Input.GetKey(Control_Config.GetKey("zoomIn", "editor")))
                ZoomIn();
            if (Input.GetKey(Control_Config.GetKey("zoomOut", "editor")))
                ZoomOut();
            if (Input.GetKey(Control_Config.GetKey("reset", "editor")))
                Reset();
        }

        void LateUpdate()
        {
            // Update rect position based on delta
            transform.Translate(m_dragDelta);

            if (!m_tracking)
                m_dragDelta *= 0.88f;
            else
                m_dragDelta = Vector2.zero;

            // Keep edges within rect of parent
            Vector2 newPos = Vector3.zero;
            float thisLeft = transform.position.x - (GetComponent<RectTransform>().rect.width * .5f);
            float parentLeft = transform.parent.position.x - (transform.parent.GetComponent<RectTransform>().rect.width * .5f);

            float thisRight = transform.position.x + (GetComponent<RectTransform>().rect.width * .5f);
            float parentRight = transform.parent.position.x + (transform.parent.GetComponent<RectTransform>().rect.width * .5f);

            float thisBottom = transform.position.y - (GetComponent<RectTransform>().rect.height * .5f);
            float parentBottom = transform.parent.position.y - (transform.parent.GetComponent<RectTransform>().rect.height * .5f);

            float thisTop = transform.position.y + (GetComponent<RectTransform>().rect.height * .5f);
            float parentTop = transform.parent.position.y + (transform.parent.GetComponent<RectTransform>().rect.height * .5f);

            if (thisLeft > parentLeft)
            {
                newPos.x = -(thisLeft - parentLeft);
            }

            if (thisRight < parentRight)
            {
                newPos.x = -(thisRight - parentRight);
            }

            if (thisBottom > parentBottom)
            {
                newPos.y = -(thisBottom - parentBottom);
            }

            if (thisTop < parentTop)
            {
                newPos.y = -(thisTop - parentTop);
            }

            transform.Translate(newPos);
        }

        #endregion

        #region PRIVATE UTILITIES

        private void Cancel()
        {
            DblClick = false;
        }

        #region ZOOM 

        private void ZoomIn()
        {
            m_zoomLevel += .1f;
            if (m_zoomLevel > 2)
                m_zoomLevel = 2;

            ApplyZoom();
        }

        private void ZoomOut()
        {
            m_zoomLevel -= .1f;
            if (m_zoomLevel < .5f)
                m_zoomLevel = .5f;

            ApplyZoom();
        }

        private void ApplyZoom()
        {
            transform.localScale = m_originalScale * m_zoomLevel;
        }

        #endregion

        private void Reset()
        {
            transform.localPosition = Vector3.zero;
            m_dragDelta = Vector2.zero;
            m_zoomLevel = 1f;
            ApplyZoom();
        }

        #endregion

        #region POINTER EVENTS

        public void OnPointerDown(PointerEventData data)
        {
            // when middle button is pressed - track delta and drag rect
            if (data.button == PointerEventData.InputButton.Middle)
            {
                // player successfully executed a double click
                if (DblClick)
                {
                    Reset();
                    DblClick = false;
                }
                // set player up for a double click
                DblClick = true;
                Invoke("Cancel", .3f);
            }
        }

        public void OnBeginDrag(PointerEventData data)
        {
            // when middle button is pressed - track delta and drag rect
            if (data.button == PointerEventData.InputButton.Middle)
            {
                m_prevMPos = data.position;
                m_tracking = true;
            }
        }

        public void OnDrag(PointerEventData data)
        {
            // when middle button is pressed - track delta and drag rect
            if (data.button == PointerEventData.InputButton.Middle)
            {
                m_dragDelta = (data.position - m_prevMPos);
                m_prevMPos = data.position;
            }
        }

        public void OnEndDrag(PointerEventData data)
        {
            if (data.button == PointerEventData.InputButton.Middle)
            {
                m_tracking = false;
                m_dragDelta = (data.position - m_prevMPos);
            }
        }

        #endregion
    }
}