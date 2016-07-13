using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Menu
{
    public class ListDividerPrefab : MonoBehaviour {

        public Text TextBox;

        public void SetDivide(string category)
        {
            TextBox.text = string.Format("Control configuration: {0}", category);
        }
    }
}
