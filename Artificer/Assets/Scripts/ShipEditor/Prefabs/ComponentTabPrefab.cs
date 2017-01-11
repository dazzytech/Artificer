using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Construction.ShipEditor
{
    public class ComponentTabPrefab : MonoBehaviour
    {
        public Text Label;
        public Button Btn;

        public void SetTab(string item, EditorListener listener)
        {
            Label.text =  item;
            Btn.onClick.AddListener(
                delegate{listener.SelectTab(item);});
        }
    }
}
