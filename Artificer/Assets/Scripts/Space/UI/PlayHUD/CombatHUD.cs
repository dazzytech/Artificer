using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using Data.Space;
using Space.Ship;

namespace Space.UI.Ship
{
    public class CombatHUD : MonoBehaviour {

        public Text HelpText;
        public Text StatusText;

    	// Use this for initialization
    	void OnEnable () {
            SpaceManager.OnPlayerUpdate += UpdateCombat;
    	}

        void OnDisable () {
            SpaceManager.OnPlayerUpdate -= UpdateCombat;
        }
    	
    	// Update is called once per frame
    	void UpdateCombat (Transform player) 
        {
            HelpText.text = string.Format("MOUSE FOLLOW MODE \n[KEY '{0}']",
                                          Control_Config.GetKey("switchtocombat", "ship").ToString());

            ShipData Ship = player.GetComponent<ShipAttributes>().Ship;

            if (Ship.CombatActive)
            { 
                StatusText.text = "ON";
                StatusText.color = new Color(1f, 0.654f, 0.654f);
            } else
            {
                StatusText.text = "OFF";
                StatusText.color = new Color(0.631f, 0.909f, 1f);
            }
    	}
    }
}