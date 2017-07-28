using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Menu
{
    public class ListKeyPrefab : MonoBehaviour
    {
        public Text ControlLabel;
        public Text KeyLabel;
        public Button Btn;

        public void AssignKeyData(KeyData key)
        {
            ControlLabel.text = key.Label;
            KeyLabel.text = KeyLibrary.SetString(key.Key.ToString());

            //transform.localScale = new Vector3(1, 1, 0);
            //transform.localPosition = new Vector3(0, 0, 0);
        }
    }
}

