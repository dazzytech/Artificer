using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Data.Space.Library;

/// <summary>
/// Component attributes.
/// A centralized storage container for all 
/// partitions of the component creator
/// </summary>
namespace Editor.Components
{
    public class ComponentAttributes
    {
        // Component Data
        public GameObject ComponentGO;
        public string ComponentLabel;
        public string ComponentName;
        // store a dictionary of sprites
        public Dictionary<string, Sprite> ComponentSprites = new Dictionary<string, Sprite>();
        public Sprite ComponentSprite;

        // ComponentObject Data
        public List<ComponentObject> Sockets;       // Can also store fire and engine points
        public EnginePoint EP;                      // Engine Point

        // each component cannot have more that 4 elements
        public string[] Symbols = new string[4];
        public float[] Amounts = new float[4];
        public float Weight;
        public float HP;
        public ComponentObject SelectedSocket;
        public int SocketCount;
        public bool GOCollider = false;

        // Mouse and Position vars
        public Vector2 MousePos;
        public Vector2 RightClickPos;
        public Vector2 CenterPoint;
        public bool Dragging = false;

        // Component Types
        public ComponentType Type = new ComponentType();
    }
}

