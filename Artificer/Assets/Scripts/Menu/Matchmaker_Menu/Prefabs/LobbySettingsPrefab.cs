using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Lobby
{
    public class LobbySettingsPrefab : MonoBehaviour
    {
        // UI Elements

        // toggle interaction
        public Transform ControlPanel;

        [SerializeField]
        private Text Label;

        [SerializeField]
        private Text Value;

        // index tracking vars 
        private string[] values;

        int index;

        /// <summary>
        /// Create lobby settings, not defined beforehand
        /// </summary>
        /// <param name="LobbyOwner"></param>
        public void BuildSettings
            (string label, string value, string[] values = null)
        {
            Label.text = label;

            Value.text = value;

            // if is multiple selection
            if (values != null)
            {
                index = 0;
                foreach (string item in values)
                {
                    if (value == item)
                    {
                        break;
                    }
                    ++index;
                }
                this.values = values;
            }
            else
                ControlPanel.gameObject.SetActive(false);
        }

        public void increment()
        {
            if (index + 1 < values.Length)
            {
                index++;
                Value.text = values[index];
            }
        }

        public void decrement()
        {
            if (index - 1 > 0)
            {
                index--;
                Value.text = values[index];
            }
        }
    }
}