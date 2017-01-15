using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using Space.Ship.Components.Listener;
using Space.Ship.Components.Attributes;

namespace Construction.ShipEditor
{
    public class TargeterCustomizerSelection : MonoBehaviour
    {
        public Transform Info;

        public Toggle AutoTargetFire;
        public Toggle SelectTargets;
        public Toggle MouseFollow;
        public int currentT;

        BaseShipComponent BSC;

        public void DisplayInfo(TargeterAttributes att, BaseShipComponent bSC)
        {
            Info.Find("Angle").GetComponent<Text>().text = (att.MaxAngle + Mathf.Abs(att.MinAngle)).ToString();
            Info.Find("Rotate").GetComponent<Text>().text = att.turnSpeed.ToString();
            Info.Find("Range").GetComponent<Text>().text = (att.AttackRange*.1f).ToString()+"km";
            
            BSC = bSC;

            DisplayState();
        }

        public void ChangeState(int trigger)
        {
            if (currentT == trigger)
                return;
            
            switch (trigger)
            {
                case 0:
                    if(AutoTargetFire.isOn == false)
                        return;
                    BSC.ShipComponent.behaviour = 0;
                    break;
                case 1:
                    if(MouseFollow.isOn == false)
                        return;
                    BSC.ShipComponent.behaviour = 1;
                    break;
                case 2:
                    if(SelectTargets.isOn == false)
                        return;
                    BSC.ShipComponent.behaviour = 2;
                    break;
            }
            
            DisplayState();
        }

        public void DisplayState()
        {
            switch(BSC.ShipComponent.behaviour)
            { 
                case 0:
                    AutoTargetFire.isOn = true;
                    SelectTargets.isOn = false;
                    MouseFollow.isOn = false;
                    currentT = 0;
                    break;
                case 1:
                    AutoTargetFire.isOn = false;
                    MouseFollow.isOn = true;
                    SelectTargets.isOn = false;
                    currentT = 1;
                    break;
                case 2:
                    AutoTargetFire.isOn = false;
                    MouseFollow.isOn = false;
                    SelectTargets.isOn = true;
                    currentT = 2;
                    break;
            }
        }
    }
}

