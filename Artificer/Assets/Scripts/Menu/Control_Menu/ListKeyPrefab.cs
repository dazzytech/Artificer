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

        public void AssignKeyData(string contLabel, string keyLabel)
        {
            ControlLabel.text = contLabel;
            KeyLabel.text = keyLabel;

            //transform.localScale = new Vector3(1, 1, 0);
            //transform.localPosition = new Vector3(0, 0, 0);
        }
    }
}

