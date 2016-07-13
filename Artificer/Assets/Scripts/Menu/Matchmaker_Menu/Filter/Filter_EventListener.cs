using UnityEngine;
using System.Collections;

namespace Menu
{
    [RequireComponent(typeof(Filter_Behaviour))]
    public class Filter_EventListener : MonoBehaviour
    {
        private Filter_Behaviour m_controller;

        void Awake()
        {
            m_controller = GetComponent<Filter_Behaviour>();
        }

        public void Confirm()
        {
            GameObject.Find("Online")
                .SendMessage("OpenLobbyListWindowFiltered", 
                             m_controller.ApplyFilter());
        }

        public void Cancel()
        {
            GameObject.Find("Online")
                .SendMessage("OpenLobbyListWindow");
        }
    }
}
