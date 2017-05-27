using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using Space.Ship.Components.Listener;
using Space.Ship.Components.Attributes;

namespace Construction.ShipEditor
{
    /// <summary>
    /// Displays the information 
    /// regading the component it is created for
    /// </summary>
    public class ComponentHoverController : MonoBehaviour 
    {
        public RawImage Icon;
        public Text Title;
        public Text Desc;
        public Text Type;
        public Text Weight;
        public Text Integrity;
        public Text ReqOne;
        public Text ReqTwo;
        public Text ReqThree;
        public Text ReqFour;

        // Panels for different rects
        public Transform EnginePanel;
        public Transform WeaponPanel;
        public Transform LauncherPanel;
        public Transform RotorPanel;
        public Transform TargeterPanel;
        public Transform ShieldPanel;
        public Transform WellPanel;
        public Transform WarpPanel;

        // Assuming all vars are assigned
        public void DisplayComp(ComponentListener Con, Texture Tex)
        {
            // set as objects tex
            Icon.texture = Tex;
            Icon.transform.localScale = new Vector3(Tex.width*.01f, Tex.height*.01f);

            ComponentAttributes att = Con.GetAttributes();
            // Set text items
            Title.text = att.Name;
            Desc.text = att.Description;
            Type.text = Con.ComponentType;

            // Assign base attributes

            if((Con.Weight * 1000)>= 1000)
                Weight.text =  Con.Weight.ToString("F1")+"Ton(m)";
            else
                Weight.text =  (Con.Weight *1000).ToString("F0")+"KG";

            Integrity.text = att.Integrity.ToString();


            // Assign amounts
            if (att.RequiredMats.Length > 0)
                ReqOne.text = string.Format("{0}: {1}", att.RequiredMats [0].amount, att.RequiredMats [0].material);
            else
                ReqOne.text = "-";

            if (att.RequiredMats.Length > 1)
                ReqTwo.text = string.Format("{0}: {1}", att.RequiredMats [1].amount, att.RequiredMats [1].material);
            else
                ReqTwo.text = "-";

            if (att.RequiredMats.Length > 2)
                ReqThree.text = string.Format("{0}: {1}", att.RequiredMats [2].amount, att.RequiredMats [2].material);
            else
                ReqThree.text = "-";

            if (att.RequiredMats.Length > 3)
                ReqFour.text = string.Format("{0}: {1}", att.RequiredMats [3].amount, att.RequiredMats [3].material);
            else
                ReqFour.text = "-";

            // Show panel to corresponding type
            switch (Con.ComponentType)
            {
                case "Engines":
                    EnginePanel.gameObject.SetActive(true);
                    EnginePanel.Find("Acceleration").GetComponent<Text>().text = ((EngineAttributes)att).acceleration.ToString("F2");
                    EnginePanel.Find("MaxSpeed").GetComponent<Text>().text = ((EngineAttributes)att).maxSpeed.ToString();
                    break;
                case "Weapons":
                    WeaponPanel.gameObject.SetActive(true);
                    WeaponPanel.Find("Damage").GetComponent<Text>().text = ((WeaponAttributes)att).WeaponDamage.ToString();
                    WeaponPanel.Find("Range").GetComponent<Text>().text = 
                        (((WeaponAttributes)att).WeaponRange*.1f).ToString() + "km";
                    WeaponPanel.Find("Delay").GetComponent<Text>().text = ((WeaponAttributes)att).WeaponDelay.ToString("F2");
                    WeaponPanel.Find("Type").GetComponent<Text>().text = ((WeaponAttributes)att).ProjectilePrefab.name.ToString();
                    break;
                case "Launchers":
                    LauncherPanel.gameObject.SetActive(true);
                    LauncherPanel.Find("Damage").GetComponent<Text>().text = ((LauncherAttributes)att).WeaponDamage.ToString();
                    LauncherPanel.Find("Range").GetComponent<Text>().text = 
                        (((LauncherAttributes)att).WeaponRange*.1f).ToString() + "km";
                    LauncherPanel.Find("Delay").GetComponent<Text>().text = ((LauncherAttributes)att).WeaponDelay.ToString("F2");
                    LauncherPanel.Find("TargetRange").GetComponent<Text>().text = 
                        (((LauncherAttributes)att).AttackRange*.1f).ToString() + "km";
                    LauncherPanel.Find("RCount").GetComponent<Text>().text = ((LauncherAttributes)att).Rockets.ToString();
                    LauncherPanel.Find("RDelay").GetComponent<Text>().text = ((LauncherAttributes)att).RocketDelay.ToString("F2");
                    break;
                case "Rotors":
                    RotorPanel.gameObject.SetActive(true);
                    RotorPanel.Find("Acceleration").GetComponent<Text>().text = ((RotorAttributes)att).turnAcceleration.ToString("F2");
                    RotorPanel.Find("MaxTurn").GetComponent<Text>().text = ((RotorAttributes)att).maxTurnSpeed.ToString();
                    break;
                case "Targeter":
                    TargeterPanel.gameObject.SetActive(true);
                    TargeterPanel.Find("Angle").GetComponent<Text>().text = 
                        (((TargeterAttributes)att).MaxAngle + Mathf.Abs(((TargeterAttributes)att).MinAngle)).ToString();
                    TargeterPanel.Find("Rotate").GetComponent<Text>().text = ((TargeterAttributes)att).turnSpeed.ToString();
                    TargeterPanel.Find("Range").GetComponent<Text>().text = (((TargeterAttributes)att).AttackRange*.1f).ToString()+"km";
                    break;
                case "Shields":
                    ShieldPanel.gameObject.SetActive(true);
                    ShieldPanel.Find("Integrity").GetComponent<Text>().text = ((ShieldAttributes)att).ShieldIntegrity.ToString();
                    ShieldPanel.Find("Radius").GetComponent<Text>().text = ((ShieldAttributes)att).ShieldRadius.ToString()+"m";
                    ShieldPanel.Find("Recharge").GetComponent<Text>().text = ((ShieldAttributes)att).RechargeDelay.ToString()+"s";
                    break;
                case "Wells":
                    WellPanel.gameObject.SetActive(true);
                    WellPanel.Find("Radius").GetComponent<Text>().text = ((CollectorAttributes)att).Radius.ToString()+"m";
                    WellPanel.Find("Force").GetComponent<Text>().text = ((CollectorAttributes)att).PullForce.ToString();
                    break;
                case "Warps":
                    WarpPanel.gameObject.SetActive(true);
                    WarpPanel.Find("WarmUp").GetComponent<Text>().text = ((WarpAttributes)att).WarpWarmUp.ToString()+"s";
                    WarpPanel.Find("CoolDown").GetComponent<Text>().text = ((WarpAttributes)att).WarpDelay.ToString()+"s";
                    WarpPanel.Find("Min").GetComponent<Text>().text = (((WarpAttributes)att).MinDistance*.1f).ToString()+"km";
                    WarpPanel.Find("Max").GetComponent<Text>().text = (((WarpAttributes)att).MaxDistance*.1f).ToString()+"km";
                    break;
                default:
                    break;
            }
        }
    }
}