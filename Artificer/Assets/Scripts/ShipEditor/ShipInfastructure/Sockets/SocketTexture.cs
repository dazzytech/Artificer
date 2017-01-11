using UnityEngine;
using System.Collections;

namespace Construction.ShipEditor
{
    /// <summary>
    /// Socket texture.
    /// central reference for accessing textures
    /// </summary>
    public class SocketTexture
    {
        public static Texture open;
        public static Texture closed;
        public static Texture pending;

        // Use this for initialization
        public SocketTexture()
        {
            open = Resources.Load("Textures/ShipEditor/socket_open", 
                                  typeof(Texture))as Texture;
            
            pending =  Resources.Load("Textures/ShipEditor/socket_pending", 
                                      typeof(Texture))as Texture;
            
            closed = Resources.Load("Textures/ShipEditor/socket_closed", 
                                    typeof(Texture))as Texture;
        }
    }
}

