using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Menu
{
    public class ResolutionListItem : MonoBehaviour {

        public Resolution resolution;
        public Button button;

        public void SetResolution(Resolution newRes)
        {
            resolution = newRes;

            GetComponentInChildren<Text>().text =
                string.Format("{0}x{1}: {2}Hz", resolution.width, resolution.height, resolution.refreshRate);
        }
    }
}
