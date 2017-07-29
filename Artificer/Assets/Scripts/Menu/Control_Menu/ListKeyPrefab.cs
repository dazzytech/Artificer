using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Menu
{
    public class ListKeyPrefab : MonoBehaviour
    {
        #region ATTRIBUTES

        [SerializeField]
        private Text m_controlLabel;
        [SerializeField]
        private Text m_keyLabel;
        public Button Btn;

        private KeyData m_data;

        #endregion

        #region ACCESSORS

        public string ID
        {
            get { return m_data.ID; }
        }

        public string Category
        {
            get { return m_data.Category; }
        }

        public string Key
        {
            get { return KeyLibrary.SetString(m_data.Key.ToString()); }
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Initializes HUD elements and stores key
        /// </summary>
        /// <param name="key"></param>
        public void AssignKeyData(KeyData key)
        {
            m_data = key;

            m_controlLabel.text = key.Label;
            m_keyLabel.text = KeyLibrary.SetString(key.Key.ToString());

            //transform.localScale = new Vector3(1, 1, 0);
            //transform.localPosition = new Vector3(0, 0, 0);
        }

        /// <summary>
        /// Display that we are waiting for a key
        /// </summary>
        public void SetPending()
        {
            m_keyLabel.text = "input pending";
        }

        #endregion
    }
}

