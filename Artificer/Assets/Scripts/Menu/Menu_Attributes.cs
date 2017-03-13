using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Menu
{
    public enum MenuState{Play, Video, Audio, Controls, Servers, Credits, Popup, None};

    public class Menu_Attributes : MonoBehaviour
    {
    	/*
    	 * Option states and variables
    	 */
        public MenuState MenuState;

        /*
         * Panels/Tabs
         */
        public GameObject VideoTab;
        public GameObject AudioTab;
        public GameObject ControlTab;
        public GameObject CreditsTab;
        public GameObject ServerTab;
        public GameObject PlayTab;
        public GameObject PopupWindow;
        public GameObject TabPanel;
        public GameObject BasePanel;

        public Text VersionText;
    }
}
