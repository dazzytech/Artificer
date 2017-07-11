using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using Space.Ship.Components.Listener;
using Space.Ship.Components.Attributes;

namespace Construction.ShipEditor
{
    public class LauncherCustomizerSelection : MonoBehaviour
    {
        /*public Transform Info;
        
        public Toggle Primary;
        public Toggle Secondary;
        public Toggle Tertiary;

        public Toggle AutoTarget;
        
        public ToggleGroup Group;
        
        BaseShipComponent BSC;
        
        int currentT;

        public void DisplayWeaponInfo(LauncherAttributes att, BaseShipComponent bSC)
        {
            Group.RegisterToggle(Primary);
            Group.RegisterToggle(Secondary);
            Group.RegisterToggle(Tertiary);

            Info.Find("Damage").GetComponent<Text>().text = att.WeaponDamage.ToString();
            Info.Find("Range").GetComponent<Text>().text = (att.WeaponRange*.1f).ToString() + "km";
            Info.Find("Delay").GetComponent<Text>().text =  att.WeaponDelay.ToString("F2");
            Info.Find("TargetRange").GetComponent<Text>().text = (att.AttackRange*.1f).ToString() + "km";
            Info.Find("RCount").GetComponent<Text>().text = att.Rockets.ToString();
            Info.Find("RDelay").GetComponent<Text>().text = att.RocketDelay.ToString("F2");
            
            BSC = bSC;
            
            DisplayTrigger();
            AutoTarget.isOn = BSC.ShipComponent.AutoLock;
        }

        public void SetButton(int trigger)
        {
            if (currentT == trigger)
                return;
            
            switch (trigger)
            {
                case 0:
                    if(Primary.isOn == false)
                        return;
                    BSC.WType = WeaponType.PRIMARY;
                    break;
                case 1:
                    if(Secondary.isOn == false)
                        return;
                    BSC.WType = WeaponType.SECONDARY;
                    break;
                case 2:
                    if(Tertiary.isOn == false)
                        return;
                    BSC.WType = WeaponType.TERTIARY;
                    break;
            }
            
            BSC.AssignKey();
            
            DisplayTrigger();
        }

        public void AssignAutoAttack()
        {
            BSC.ShipComponent.AutoLock = AutoTarget.isOn;
        }
        
        public void DisplayTrigger()
        {
            switch(BSC.WType)
            { 
                case WeaponType.PRIMARY:
                    Primary.isOn = true;
                    Secondary.isOn = false;
                    Tertiary.isOn = false;
                    currentT = 0;
                    break;
                case WeaponType.SECONDARY:
                    Primary.isOn = false;
                    Secondary.isOn = true;
                    Tertiary.isOn = false;
                    currentT = 1;
                    break;
                case WeaponType.TERTIARY:
                    Primary.isOn = false;
                    Secondary.isOn = false;
                    Tertiary.isOn = true;
                    currentT = 2;
                    break;
            }
        }*/
    }
}
