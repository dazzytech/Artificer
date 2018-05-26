using Data.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Space.UI.IDE
{
    /// <summary>
    /// 
    /// </summary>
    public class NodeCreatePrefab : MonoBehaviour,
        IPointerDownHandler, IPointerEnterHandler,
        IPointerExitHandler
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
        private NodeData m_nodeData;

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

        /// <summary>
        /// Creates image and assigns delegate
        /// and component prefab
        /// </summary>
        /// <param name="GO"></param>
        /// <param name="create"></param>
        public void CreateItem(NodeData node,
            EditorManager.Create create)
        {
            m_nodeData = node;

            m_hovering = false;

            m_create = create;

            // Assign the data to our visual elements
            m_label.text = node.Label;

            foreach(NodeData.IO input in node.Input)
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

        #region POINTER DATA

        public void OnPointerDown(PointerEventData data)
        {
            // Create the prefab gameobject and assign with the node data

            if (data.button == PointerEventData.InputButton.Left)
                m_create(null);
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
