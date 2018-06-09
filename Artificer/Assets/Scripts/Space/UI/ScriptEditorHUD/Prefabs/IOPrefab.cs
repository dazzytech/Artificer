using Data.UI;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Space.UI.IDE
{
    /// <summary>
    /// Handles the visual
    /// elements of the IO
    /// </summary>
    public class IOPrefab : MonoBehaviour
    {
        #region ATTRIBUTES

        #region UI ELEMENTS

        [SerializeField]
        private Text m_label;

        private RawImage m_image;

        [SerializeField]
        private RawImage m_imgIn;

        [SerializeField]
        private RawImage m_imgOut;

        [SerializeField]
        private InputField m_input;

        [SerializeField]
        private Toggle m_toggle;

        #endregion

        private NodeData.IO m_data;

        private bool m_isInput;

        #region ICONS

        [Header("Icon Images")]

        [SerializeField]
        private Texture2D m_openIO;

        [SerializeField]
        private Texture2D m_closedIO;

        #endregion

        #region COLOURS

        [Header("Colours")]

        [SerializeField]
        private Color m_undefColour;

        [SerializeField]
        private Color m_numColour;

        [SerializeField]
        private Color m_stringColour;

        [SerializeField]
        private Color m_boolColour;

        [SerializeField]
        private Color m_objectColour;

        [SerializeField]
        private Color m_numArrayColour;

        [SerializeField]
        private Color m_stringArrayColour;

        [SerializeField]
        private Color m_objectArrayColour;

        [SerializeField]
        private Color m_execColour;

        #endregion

        #endregion

        #region ACCESSORS

        public string Label
        {
            get
            {
                return m_label.text;
            }
            set
            {
                m_label.text = value;
            }
        }

        public bool In
        {
            get
            {
                return m_isInput;
            }
        }

        public RectTransform IconBounds
        {
            get
            {
                return m_image.rectTransform;
            }
        }

        public NodeData.IO IO
        {
            get
            {
                return m_data;
            }
        }

        public Color Colour
        {
            get
            {
                return m_image.color;
            }
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// sets the colour and image of the node
        /// and updates the UI elements based on the variable type
        /// </summary>
        /// <param name="data"></param>
        public void Initialize(NodeData.IO data, bool input)
        {
            m_data = data;

            Label = m_data.Label;

            m_isInput = input;

            DisableInputs();

            // assign the image to in or out based on input or output
            if (m_isInput)
            {
                m_image = m_imgIn;
                m_imgIn.gameObject.SetActive(true);
                m_imgOut.gameObject.SetActive(false);
            }
            else
            {
                m_image = m_imgOut;
                m_imgIn.gameObject.SetActive(false);
                m_imgOut.gameObject.SetActive(true);              
            }

            UpdateNode();
        }

        /// <summary>
        /// Changes visual based on node data
        /// </summary>
        public void UpdateNode()
        {
            ColourNode();

            // update IO texture based on linkID
            if (m_data.LinkedIO == null)
                m_image.texture = m_openIO;
            else
            {
                m_image.texture = m_closedIO;
            }
        }

        /// <summary>
        /// Updates the input fields for user input
        /// </summary>
        public void UpdateInput()
        {
            DisableInputs();
            if (m_data.LinkedIO == null)
            {
                if (m_isInput)
                {
                    switch (m_data.CurrentType)
                    {
                        case NodeData.IO.IOType.NUM:
                            m_input.gameObject.SetActive(true);
                            m_input.contentType = InputField.ContentType.DecimalNumber;
                            break;
                        case NodeData.IO.IOType.STRING:
                            m_input.gameObject.SetActive(true);
                            m_input.contentType = InputField.ContentType.Alphanumeric;
                            break;
                        case NodeData.IO.IOType.BOOL:
                            m_toggle.gameObject.SetActive(true);
                            InputToggleChanged();
                            break;
                    }
                }
            }
        }

        public void Close()
        {
            m_image.texture = m_closedIO;
        }

        public void Open()
        {
            m_image.texture = m_openIO;
        }

        #endregion

        #region EVENT LISTENER

        public void InputTextChanged()
        {
            if(m_data.CurrentType == NodeData.IO.IOType.STRING)
                m_data.Value = string.Format("\"{0}\"", m_input.text);
            else
                m_data.Value = m_input.text;
        }

        public void InputToggleChanged()
        {
            m_data.Value = m_toggle.isOn ? "true" : "false";
        }

        #endregion

        #region PRIVATE UTILITIES

        /// <summary>
        /// Changes the colour depending on the variable
        /// type
        /// </summary>
        private void ColourNode()
        {
            if (m_data.Type == NodeData.IO.IOType.LINK)
                m_image.color = m_execColour;
            else
            {
                // assign icon based on type
                switch (m_data.CurrentType)
                {
                    case NodeData.IO.IOType.UNDEF:
                    case NodeData.IO.IOType.UNDEFSINGLE:
                        m_image.color = m_undefColour;
                        break;
                    case NodeData.IO.IOType.NUM:
                        m_image.color = m_numColour;
                        break;
                    case NodeData.IO.IOType.STRING:
                        m_image.color = m_stringColour;
                        break;
                    case NodeData.IO.IOType.BOOL:
                        m_image.color = m_boolColour;
                        break;
                    case NodeData.IO.IOType.OBJECT:
                        m_image.color = m_objectColour;
                        break;
                    case NodeData.IO.IOType.OBJARRAY:
                        m_image.color = m_objectArrayColour;
                        break;
                    case NodeData.IO.IOType.STRINGARRAY:
                        m_image.color = m_stringArrayColour;
                        break;
                    case NodeData.IO.IOType.NUMARRAY:
                        m_image.color = m_numArrayColour;
                        break;
                }
            }
        }

        private void DisableInputs()
        {
            m_toggle.gameObject.SetActive(false);
            m_input.gameObject.SetActive(false);
        }


        #endregion
    }
}