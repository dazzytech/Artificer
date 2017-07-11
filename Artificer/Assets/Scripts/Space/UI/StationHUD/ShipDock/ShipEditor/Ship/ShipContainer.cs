using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
// Artificer
using Data.Shared;
using Space.UI.Station.Editor.Component;

namespace Space.UI.Station.Editor
{
    /// <summary>
    /// Data object for storing 
    /// ship data with editor
    /// </summary>
    public class ShipContainer
    {
        [HideInInspector]
        /// <summary>
        /// Data object that constructs our ship
        /// </summary>
        public ShipData Ship;

        [HideInInspector]
        /// <summary>
        /// Stores all components of the ship including the head.
        /// </summary>
        public List<BaseComponent> Components = new List<BaseComponent>();

        [HideInInspector]
        /// <summary>
        /// Reference of components that are added to stop dups
        /// </summary>
        public List<int> AddedIDs = new List<int>();

        [HideInInspector]
        /// <summary>
        /// Socket links are stored seperately in shipdata
        /// </summary>
        public Dictionary<SocketData, BaseComponent> Links = new Dictionary<SocketData, BaseComponent>();

        [HideInInspector]
        /// <summary>
        /// Ref to the head component of the ship
        /// </summary>
        public BaseComponent Head;
    }
}

