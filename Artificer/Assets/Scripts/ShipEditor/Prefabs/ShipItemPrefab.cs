using UnityEngine;
using UnityEngine.UI;
using System.Collections;
// Artificer
using Data.Shared;
namespace Construction.ShipEditor
{
    public class ShipItemPrefab : MonoBehaviour
    {
        public RawImage ShipImage;
        public Button SelectionButton;

        public void SetShipItem(ShipData ship, EditorListener listener)
        {
            // Set the image to the ship icon
            /*Texture2D tex = ship.IconTex;

            if (tex != null)
            {
                ShipImage.texture = tex;
                ShipImage.transform.localScale = new Vector3(tex.width * 0.002f, tex.height * 0.002f);
            }

            SelectionButton.onClick.AddListener(delegate{listener.SelectShip(ship);});*/
        }
    }
}
