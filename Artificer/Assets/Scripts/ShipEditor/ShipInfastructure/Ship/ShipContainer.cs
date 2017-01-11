using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
// Artificer
using Data.Shared;

namespace Construction.ShipEditor
{
    /// <summary>
    /// Ship container - stores list of components and 
    /// manages them e.g. loading component data into the items
    /// 
    /// </summary>
    public class ShipContainer
    {
        [HideInInspector]
        // Shared data object
        public ShipData Ship;

        [HideInInspector]
        // Stores all components of the ship including the head.
        public List<BaseShipComponent> Components;

        [HideInInspector]
        public List<int> AddedIDs;

        [HideInInspector]
        public Dictionary<Socket, BaseShipComponent> Links;

        [HideInInspector]
        // reference to the ships head
        public BaseShipComponent Head;
      
        // stores the socket texture object
        private SocketTexture socketTex;

        // pre use utility actions
        public ShipContainer()
        {
            Components = new List<BaseShipComponent>();
            AddedIDs = new List<int>();
            socketTex = new SocketTexture();
            Links = new Dictionary<Socket, BaseShipComponent>();
        }
    }
}

