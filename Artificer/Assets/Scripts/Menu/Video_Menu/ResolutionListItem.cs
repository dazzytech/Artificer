using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

namespace Menu
{
    public class ResolutionListItem : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler
    {

        public Resolution resolution;
        public Button button;

        private Video_EventListener m_listener;

        public void SetResolution(Resolution newRes, Video_EventListener listener)
        {
            m_listener = listener;
            resolution = newRes;

            GetComponentInChildren<Text>().text =
                string.Format("{0}x{1}: {2}Hz", resolution.width, resolution.height, resolution.refreshRate);
        }

        public void OnPointerEnter(PointerEventData data)
        {
            GetComponent<Image>().color = new Color(.11f, .48f, .45f);
        }

        public void OnPointerExit(PointerEventData data)
        {
            if(!m_listener.Selected.Equals(this) || m_listener.Selected == null)
                GetComponent<Image>().color = new Color(0, 0, 0);
            else
                GetComponent<Image>().color = new Color(.8f, .2f, .2f, .5f);
        }
    }
}
