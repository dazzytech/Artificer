using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Menu
{
    public class Control_Behaviour : MonoBehaviour
    {
        #region ATTRIBUTES

        // Key list item prefab
        public GameObject KeyItemPrefab;
        // list divider prefab
        public GameObject KeyDividerPrefab;
        // Key List
        public Transform KeyList;
        
        // event listener
        private Control_Eventlistener m_listener;

        #endregion

        #region MONO BEHAVIOUR

        // Use this for initialization
        void Start ()
        {
            // Assign listener obj
            m_listener = GetComponent<Control_Eventlistener>();

            Redraw();
    	}

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Creates the button from key prefab.
        /// </summary>
        /// <returns>The button.</returns>
        /// <param name="res">Res.</param>
        public void CreateButton(KeyData key)
        {
            GameObject KeyBtn = Instantiate(KeyItemPrefab) as GameObject;
            KeyBtn.transform.SetParent(KeyList);
            KeyBtn.transform.localScale = new Vector3(1, 1, 1);
            KeyBtn.transform.localPosition = new Vector3(0, 0, 0);

            ListKeyPrefab Key = KeyBtn.GetComponent<ListKeyPrefab>();
            Key.AssignKeyData(key);
            Key.Btn.
               onClick.AddListener(
                    delegate{m_listener.SetNewKey(Key);});
        }

        public void CreateDivider(string category)
        {
            GameObject DivBtn = Instantiate(KeyDividerPrefab) as GameObject;
            DivBtn.transform.SetParent(KeyList);
            DivBtn.transform.localScale = new Vector3(1, 1, 1);
            DivBtn.transform.localPosition = new Vector3(0, 0, 0);
            
            ListDividerPrefab Div = DivBtn.GetComponent<ListDividerPrefab>();
            Div.SetDivide(category);
        }

        /// <summary>
        /// Redraw The key config list.
        /// </summary>
        public void Redraw()
        {
            // Build the resolution list using all possible resolutions
            if (KeyList != null)
            {
                // Clear current list
                foreach (Transform child in KeyList.transform) 
                {
                    GameObject.Destroy(child.gameObject);
                }

                // build ship controls
                CreateDivider("Ship");

                foreach(KeyData key in Control_Config.GetKeyList("ship"))
                {
                    CreateButton(key);
                }
                // build foot controls
                CreateDivider("Combat");
                foreach(KeyData key in Control_Config.GetKeyList("combat"))
                {
                    CreateButton(key);
                }
                CreateDivider("Ship Editor");
                foreach (KeyData key in Control_Config.GetKeyList("editor"))
                {
                    CreateButton(key);
                }
                // build ship controls
                CreateDivider("System");
                foreach(KeyData key in Control_Config.GetKeyList("sys"))
                {
                    CreateButton(key);
                }
            }
        }

        #endregion
    }
}
