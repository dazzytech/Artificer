using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

using Space.Ship.Components.Listener;
using Space.Ship.Components.Attributes;

namespace Construction.ShipEditor
{
    public class StyleController : MonoBehaviour
    {
        private List<Toggle> styleChoices;
        public ToggleGroup Group;
        public GameObject TogglePrefab;
        BaseShipComponent BSC;

        public void DisplayInfo(ComponentAttributes att, BaseShipComponent bSC)
        {
            styleChoices = new List<Toggle>();

            BSC = bSC;

            // create list of toggles for each colour
            foreach (StyleInfo info in att.componentStyles)
            {
                GameObject newToggle = Instantiate(TogglePrefab);
                newToggle.transform.SetParent(transform);

                Toggle tog = newToggle.transform
                    .Find("HeadToggle").GetComponent<Toggle>();

                newToggle.transform.Find("HeadToggle")
                    .Find("Label").GetComponent<Text>().text = info.name;

                Group.RegisterToggle(tog);

                string name = info.name;

                tog.onValueChanged.AddListener(delegate {SetStyle(name, tog);});

                if(bSC.Style == info.name)
                    tog.isOn = true;

                styleChoices.Add(tog);
            }
        }

        public void SetStyle(string style, Toggle tog)
        {
            if(BSC.Style == style)
            {
                if(!tog.isOn)
                    tog.isOn = true;
                return;
            }

            if (!tog.isOn)
                return;

            BSC.Style = style;

            foreach (Toggle other in styleChoices)
            {
                if(tog.Equals(other))
                    continue;

                other.isOn = false;
            }
        }
    }
}
