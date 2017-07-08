using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
// Artificer
using Data.Shared;

namespace Space.UI.Station.Editor
{
    /// <summary>
    /// Data object for storing 
    /// ship data with editor
    /// </summary>
    public class ShipContainer
    {
        [HideInInspector]
        // Shared data object
        public ShipData Ship;

        [HideInInspector]
        // Stores all components of the ship including the head.
        public List<BaseComponent> Components;

        [HideInInspector]
        public List<int> AddedIDs;

        [HideInInspector]
        public Dictionary<Socket, BaseComponent> Links;

        [HideInInspector]
        // reference to the ships head
        public BaseComponent Head;
      
        // stores the socket texture object
        private SocketTexture m_socketTex;

        // pre use utility actions
        public ShipContainer()
        {
            Components = new List<BaseComponent>();
            AddedIDs = new List<int>();
            m_socketTex = new SocketTexture();
            Links = new Dictionary<Socket, BaseComponent>();
        }
    }
}

