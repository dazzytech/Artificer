using Data.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Space.UI.IDE
{
    /// <summary>
    /// Creates an instance of Node Prefab when clicked
    /// </summary>
    public class NodeCreatePrefab : NodeViewer,
        IPointerDownHandler, IPointerEnterHandler,
        IPointerExitHandler
    {
        #region ATTRIBUTES

        #region REFERENCE

        /// <summary>
        /// The delegate function for 
        /// when the object is clicked
        /// </summary>
        private EditorManager.Create m_create;

        #endregion

        #region HOVERING 

        [Header("Hovering")]
        /// <summary>
        /// Prefab to be created when 
        /// mouse over
        /// </summary>
        [SerializeField]
        private GameObject m_hoverPrefab;

        /// <summary>
        /// Window instance that appears
        /// when mouse hovers over
        /// </summary>
        private GameObject m_hoverWindow;

        /// <summary>
        /// Whether of not the mouse is over
        /// </summary>
        private bool m_hovering;

        #endregion

        #endregion

        #region MONO BEHAVIOUR

        void Update()
        {
            if (m_hovering && m_hoverWindow != null)
            {
                m_hoverWindow.transform.position = Input.mousePosition
                    + new Vector3(-m_hoverWindow.GetComponent<RectTransform>().rect.width * .5f - 10,
                                  m_hoverWindow.GetComponent<RectTransform>().rect.height * .5f + 10, 1);

                // Keep edges within rect of parent
                Vector2 newPos = Vector3.zero;
                float thisLeft = m_hoverWindow.transform.position.x;

                float parentLeft = GameObject.Find("_gui").transform.position.x;

                float thisRight = m_hoverWindow.transform.position.x +
                    (m_hoverWindow.GetComponent<RectTransform>().rect.width * .5f);

                float parentRight = GameObject.Find("_gui").transform.GetComponent<RectTransform>().rect.width;

                float thisBottom = m_hoverWindow.GetComponent<RectTransform>().rect.height;

                float parentBottom = GameObject.Find("_gui").transform.position.y -
                    (GameObject.Find("_gui").transform.GetComponent<RectTransform>().rect.height * .5f);

                float thisTop = transform.position.y + 40; // upper buffer zone

                float parentTop = GameObject.Find("_gui").transform.position.y;

                if (thisLeft < parentLeft)
                {
                    newPos.x = -(thisLeft - parentLeft);
                }

                if (thisRight > parentRight)
                {
                    newPos.x = -(thisRight - parentRight);
                }

                if (thisBottom < parentBottom)
                {
                    newPos.y = -(thisBottom - parentBottom);
                }

                if (thisTop > parentTop)
                {
                    newPos.y = -(thisTop - parentTop);
                }

                m_hoverWindow.transform.Translate(newPos);
            }
        }

        #endregion

        #region PUBLIC INTERACTION

        public void CreatePrefab(NodeData node,
            EditorManager.Create create)
        {
            m_hovering = false;

            m_create = create;

            m_nodeData = node;

            DisplayItem();
        }

        #endregion

        #region POINTER DATA

        public void OnPointerDown(PointerEventData data)
        {
            // Create the prefab gameobject and assign with the node data

            if (data.button == PointerEventData.InputButton.Left)
                m_create(m_nodeData);
            if (m_hovering)
            {
                GameObject.Destroy(m_hoverWindow);
                m_hovering = false;
            }
        }

        public void OnPointerEnter(PointerEventData data)
        {
            if (!m_hovering && !Input.GetMouseButton(0))

            {
                // Create a hover window with Component data
                m_hoverWindow = Instantiate(m_hoverPrefab);
                m_hoverWindow.transform.SetParent(GameObject.Find("_gui").transform);
                m_hoverWindow.transform.localPosition = Vector3.zero;

                /// Display the details in a text popup
                m_hoverWindow.GetComponent<NodeHoverPrefab>().Display
                    (m_nodeData);

                m_hovering = true;
            }
        }

        public void OnPointerExit(PointerEventData data)
        {
            if (m_hovering)
            {
                GameObject.Destroy(m_hoverWindow);
                m_hovering = false;
            }
        }

        #endregion
    }
}
