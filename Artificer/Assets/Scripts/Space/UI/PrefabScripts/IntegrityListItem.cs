using UnityEngine;
using UnityEngine.UI;
using System.Collections;
// Artificer Defined
using Space.Ship.Components.Listener;

namespace Space.UI
{
    public class IntegrityListItem : MonoBehaviour
    {
        public Button button;
        public Text Status;
        public ComponentListener Component;
    }
}
