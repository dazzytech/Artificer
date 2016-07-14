using UnityEngine;
using UnityEngine.UI;
using System.Collections;
// Artificer
using Data.Shared;

namespace Menu
{
    public class ShipItemPrefab : MonoBehaviour
    {
        public Text Name;
        public Text Description;
        public Text Requirements;
        public RawImage Image;
        public Button Build;
        public Button Select;
        public Image Panel;

        private ShipData _ship;

        public void SetShipData(ShipData ship, SinglePlayer_EventListener listener)
        {
            _ship = ship;

            if (listener != null)
            {
                Build.onClick.AddListener(
                delegate
                {
                    listener.BuildShip(ship.Name);
                });
            
                Select.onClick.AddListener(
                delegate
                {
                    listener.SelectShip(ship.Name);
                });
            }

            Name.text = ship.Name;
            Description.text = ship.Description;
            Select.interactable = true;
            Build.interactable = true;

            // Add all materals in components together to create 
            // requirements
            Requirements.text = "";

            foreach (string mat in ship.GetRequirements().Keys)
            {
                Requirements.text += string.Format("{0} - {1}ft³ | ", mat, ship.GetRequirements()[mat]);
            }

            //Texture tex = ship.IconTex;

            /*if (tex != null)
            {
                Image.texture = tex;
                Image.transform.localScale = new Vector3(tex.width * 0.002f, tex.height * 0.002f);
            }*/

            
            /*if (ship.Equals(GameManager.GetPlayer.Ship))
            {
                // change to gold
                Select.interactable = false;
                Build.interactable = false;

                Panel.color = new Color (0.712f, 0.305f, 0f, 0.309f);
            }
            else if (GameManager.GetPlayer.ShipList.Contains(ship))
            {
                Build.interactable = false;
                Panel.color = new Color (0.796f, 1f, 0.89f, 0.309f);
            } else
            {
                Select.interactable = false;
                Panel.color = new Color(0.368f, 0.466f, 0.415f, 0.309f);
            }
            */
        }

        public void Refresh()
        {
            SetShipData(_ship, null);
        }
    }
}
