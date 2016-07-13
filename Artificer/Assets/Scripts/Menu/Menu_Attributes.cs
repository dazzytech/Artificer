using UnityEngine;
using System.Collections;

namespace Menu
{
    public enum MenuState{Video, Audio, Controls, Servers, Matchmaker, Credits, Popup, None};

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
        public GameObject MatchmakerTab;
        public GameObject PopupWindow;
        public GameObject TabPanel;

    	/*
         * Popup attributes
         */
    }
}
