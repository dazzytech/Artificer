using UnityEngine;
using System.Collections;

namespace Menu
{
    public class Tab_EventListener : MonoBehaviour
    {
        private Menu_Behaviour _controller;

        void Awake()
        {
            // retrieve attributes from parents
            _controller = transform.parent.GetComponent<Menu_Behaviour>();
        }

        /// <summary>
        /// Switchs to user selected tab.
        /// </summary>
        /// <param name="state">State.</param>
        public void SwitchToTab(string state)
        {
            switch (state)
            {
                case "MM":
                    _controller.CurrentState = MenuState.Matchmaker;
                    break;
                case "ser":
                    _controller.CurrentState = MenuState.Servers;
                    break;
                case "vid":
                    _controller.CurrentState = MenuState.Video;
                    break;
                case "aud":
                    _controller.CurrentState = MenuState.Audio;
                    break;
                case "con":
                    _controller.CurrentState = MenuState.Controls;
                    break;
                case "ced":
                    _controller.CurrentState = MenuState.Credits;
                    break;
                default:
                    _controller.CurrentState = MenuState.None;
                    break;
            }
        }

        public void QuitApplication()
        {
            _controller.CurrentState = MenuState.None;

            Application.Quit();
        }
    }
}
