using UnityEngine;
using UnityEngine.UI;
using System.Collections;
// Artificer
using Data.Shared;
namespace Construction.ShipEditor
{
    public class NewItemPrefab : MonoBehaviour
    {
        public Button Button;
        
        public void SetNewItem(EditorListener listener)
        {
            // Assign button function
            // Look out for same problem with ship selector
            Button.onClick.AddListener(delegate{listener.CreateNewShip();});
        }
    }
}
