using UnityEngine;
using UnityEngine.UI;
using System.Collections;
// Artificer
using Data.Shared;

namespace Menu
{
    public class MaterialListItem : MonoBehaviour
    {
        public Button button;
        [SerializeField]
        private Text Name;
        [SerializeField]
        private Text Symbol;
        [SerializeField]
        private Text Amount;

        private MaterialData Item;

        public void SetMaterial(MaterialData mat)
        {
            Item = mat;
            Name.text = mat.Name;
            Symbol.text = mat.Element;
        }

        public void SetAmount(float value)
        {
            if((value * Item.Density) > 1000)
                Amount.text = ((value * Item.Density)*0.001).ToString("F1")+"Ton(m)";
            else
                Amount.text = (value * Item.Density).ToString("F1")+"KG";

            Amount.text += " - " + value.ToString("F1") + "ft³";
        }

        public void SetPlayerAmount(float value)
        {
            if((value * Item.Density) > 1000)
                Amount.text += "\n/"+((value * Item.Density)*0.001).ToString("F1")+"Ton(m)";
            else
                Amount.text += "\n/"+ (value * Item.Density).ToString("F1")+"KG";
            
            Amount.text += " - " + value.ToString("F1") + "ft³";
        }

        /*public bool MatIs(MaterialData mat)
        {
            return (mat == Item);
        }*/

        public MaterialData Mat
        {
            get { return Item; }
        }
    }
}

