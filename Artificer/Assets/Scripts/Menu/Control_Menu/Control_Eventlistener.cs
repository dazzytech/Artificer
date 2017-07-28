using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Menu
{
    [RequireComponent(typeof(Control_Behaviour))]

    public class Control_Eventlistener : MonoBehaviour
    {
        // Controller
        private Control_Behaviour _controller;

        // track if we are waiting for a key
        private bool _keyPending = false;

        // label of the key awating input
        private string _keyToChange = "";

        // key category e.g ship
        private string _keyCategory = "";

        private Text _pendingText = null;

        void Awake()
        {
            _controller = GetComponent<Control_Behaviour>();
        }
    	
    	// Update is called once per frame
    	void Update () 
        {
    	
    	}

        /// <summary>
        /// Reverts to default
        /// control configuration.
        /// </summary>
        public void RevertToDefault()
        {
            Control_Config.ReturnToDefaults();

            _controller.Redraw();
        }

        /// <summary>
        /// Sets the new key.
        /// </summary>
        public void SetNewKey(ListKeyPrefab keyData, string cat)
        {
            // test we are not currently attempting to change a key
            if (!_keyPending)
            {
                // store the key and type within pending attributes.
                _keyToChange = keyData.ControlLabel.text;
                _keyCategory = cat;

                // change the label within the button to pending message
                _pendingText = keyData.KeyLabel;
                _pendingText.text = "input pending";

                // start coroutine that waits till key input.
                _keyPending = true;
                StartCoroutine("WaitForKey");
            }
        }

        IEnumerator WaitForKey()
        {
            while (!Input.anyKey) {
                yield return null;
            }
            _keyPending = false;
            if (!Input.GetKeyDown(KeyCode.Escape))
            {
                // Filter out mouse clicks
                KeyCode key = KeyLibrary.FindKeyPressed();

                if(key != KeyCode.None)
                    Control_Config.SetNewKey
                    (_keyToChange, KeyLibrary.FindKeyPressed(),
                         _keyCategory);
            }

            _pendingText.text = Control_Config.GetKey(_keyToChange,_keyCategory).ToString();

            yield return null;
        }
    }
}
