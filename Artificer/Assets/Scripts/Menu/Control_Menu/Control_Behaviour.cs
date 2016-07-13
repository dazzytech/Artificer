using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Menu
{
    public class Control_Behaviour : MonoBehaviour
    {
        // Key list item prefab
        public GameObject KeyItemPrefab;
        // list divider prefab
        public GameObject KeyDividerPrefab;
        // Key List
        public Transform KeyList;
        
        // event listener
        private Control_Eventlistener _listener;
        
        void Awake()
        {
            // Assign listener obj
            _listener = GetComponent<Control_Eventlistener>();
        }

    	// Use this for initialization
    	void Start () {
            Redraw();
    	}
    	
    	// Update is called once per frame
    	void Update () {
    	
    	}

        /// <summary>
        /// Creates the button from key prefab.
        /// </summary>
        /// <returns>The button.</returns>
        /// <param name="res">Res.</param>
        public void CreateButton(string label, string key, string cat)
        {
            GameObject KeyBtn = Instantiate(KeyItemPrefab) as GameObject;
            KeyBtn.transform.SetParent(KeyList);
            KeyBtn.transform.localScale = new Vector3(1, 1, 1);
            KeyBtn.transform.localPosition = new Vector3(0, 0, 0);

            ListKeyPrefab Key = KeyBtn.GetComponent<ListKeyPrefab>();
            Key.AssignKeyData(label, key);
            Key.Btn.
               onClick.AddListener(
                    delegate{_listener.SetNewKey(Key, cat);});
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
                foreach(string key in Control_Config.GetKeyList("ship").Keys)
                {
                    CreateButton(key, Control_Config.GetKeyList("ship")[key].ToString(), "ship");
                }
                // build foot controls
                CreateDivider("Combat");
                foreach(string key in Control_Config.GetKeyList("combat").Keys)
                {
                    CreateButton(key, Control_Config.GetKeyList("combat")[key].ToString(), "combat");
                }
                // build ship controls
                CreateDivider("System");
                foreach(string key in Control_Config.GetKeyList("sys").Keys)
                {
                    CreateButton(key, Control_Config.GetKeyList("sys")[key].ToString(), "sys");
                }
            }
        }
    }
}
