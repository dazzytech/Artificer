using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using Space.Ship.Components.Listener;
using Space.Ship.Components.Attributes;
using Space.UI.Station.Editor.Component;

namespace Space.UI.Station.Prefabs
{
    /// <summary>
    /// Displays the information 
    /// regading the component it is created for
    /// </summary>
    public class ComponentHoverPrefab : ComponentViewerPrefab
    {
        #region ATTRIBUTES

        #region HUD ELEMENTS

        [Header("Hover HUD Elements")]
        [SerializeField]
        private RawImage m_icon;
        [SerializeField]
        private Text m_description;

        #region REQUIREMENTS

        [Header("Requirement Panels")]
        [SerializeField]
        private Text m_reqOne;
        [SerializeField]
        private Text m_reqTwo;
        [SerializeField]
        private Text m_reqThree;
        [SerializeField]
        private Text m_reqFour;

        #endregion

        #endregion

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// When a hover window is made 
        /// then display all information
        /// regarding component
        /// </summary>
        /// <param name="Con"></param>
        /// <param name="Tex"></param>
        public override void Display(GameObject GO)
        {
            base.Display(GO);

            // Retreive texture for our image
            Texture2D texture = GetTexture.Get(GO);

            if (texture != null)
                AssignTexture(texture);

            ComponentListener con = GO.GetComponent<ComponentListener>();

            if (con != null)
            {
                ComponentAttributes att = con.GetAttributes();

                if (att != null)
                {
                    m_description.text = att.Description;

                    AssignRequirements(att);

                    AssignType(con.ComponentType, att);
                }
            }
        }

        #endregion

        #region PRIVATE UTILITIES

        /// <summary>
        /// Displays texture on icon image
        /// </summary>
        /// <param name="texture"></param>
        private void AssignTexture(Texture2D texture)
        {
            // set as objects tex
            m_icon.texture = texture;
            m_icon.SetNativeSize();

            float maxAngle = Mathf.Max(m_icon.texture.width, m_icon.texture.height);

            if (maxAngle > 75)
            {
                float sizeScale = 75f / maxAngle;

                m_icon.transform.localScale = new Vector3(sizeScale, sizeScale, 1);
            }
        }

        /// <summary>
        /// Displays how much the player will need
        /// to include component on their ship
        /// </summary>
        /// <param name="att"></param>
        private void AssignRequirements(ComponentAttributes att)
        {
            if (att.RequiredMats.Length > 0)
                m_reqOne.text = string.Format("{0}: {1}", att.RequiredMats[0].amount, att.RequiredMats[0].material);
            else
                m_reqOne.text = "-";

            if (att.RequiredMats.Length > 1)
                m_reqTwo.text = string.Format("{0}: {1}", att.RequiredMats[1].amount, att.RequiredMats[1].material);
            else
                m_reqTwo.text = "-";

            if (att.RequiredMats.Length > 2)
                m_reqThree.text = string.Format("{0}: {1}", att.RequiredMats[2].amount, att.RequiredMats[2].material);
            else
                m_reqThree.text = "-";

            if (att.RequiredMats.Length > 3)
                m_reqFour.text = string.Format("{0}: {1}", att.RequiredMats[3].amount, att.RequiredMats[3].material);
            else
                m_reqFour.text = "-";
        }

        /// <summary>
        /// Adds additional panels dependant on
        /// component type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="att"></param>
        protected override void AssignType(string type, ComponentAttributes att)
        {
            base.AssignType(type, att);

            switch (type)
            {
                case "Weapons":
                    m_weaponPanel.gameObject.SetActive(true);
                    m_weaponPanel.Find("Damage").GetComponent<Text>().text = ((WeaponAttributes)att).WeaponDamage.ToString();
                    m_weaponPanel.Find("Range").GetComponent<Text>().text =
                        (((WeaponAttributes)att).WeaponRange * .1f).ToString() + "km";
                    m_weaponPanel.Find("Delay").GetComponent<Text>().text = ((WeaponAttributes)att).WeaponDelay.ToString("F2");
                    m_weaponPanel.Find("Type").GetComponent<Text>().text = ((WeaponAttributes)att).ProjectilePrefab.name.ToString();
                    break;
                case "Launchers":
                    m_launcherPanel.gameObject.SetActive(true);
                    m_launcherPanel.Find("Damage").GetComponent<Text>().text = ((LauncherAttributes)att).WeaponDamage.ToString();
                    m_launcherPanel.Find("Range").GetComponent<Text>().text =
                        (((LauncherAttributes)att).WeaponRange * .1f).ToString() + "km";
                    m_launcherPanel.Find("Delay").GetComponent<Text>().text = ((LauncherAttributes)att).WeaponDelay.ToString("F2");
                    m_launcherPanel.Find("TargetRange").GetComponent<Text>().text =
                        (((LauncherAttributes)att).AttackRange * .1f).ToString() + "km";
                    m_launcherPanel.Find("RCount").GetComponent<Text>().text = ((LauncherAttributes)att).Rockets.ToString();
                    m_launcherPanel.Find("RDelay").GetComponent<Text>().text = ((LauncherAttributes)att).RocketDelay.ToString("F2");
                    break;
                case "Targeter":
                    m_targeterPanel.gameObject.SetActive(true);
                    m_targeterPanel.Find("Angle").GetComponent<Text>().text =
                        (((TargeterAttributes)att).MaxAngle + Mathf.Abs(((TargeterAttributes)att).MinAngle)).ToString();
                    m_targeterPanel.Find("Rotate").GetComponent<Text>().text = ((TargeterAttributes)att).turnSpeed.ToString();
                    m_targeterPanel.Find("Range").GetComponent<Text>().text = (((TargeterAttributes)att).AttackRange * .1f).ToString() + "km";
                    break;
                default:
                    break;
            }
        }

        #endregion
    }
}