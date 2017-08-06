using UnityEngine;
using System.Collections;

namespace Space.UI.Station.Editor.Socket
{
    /// <summary>
    /// Socket texture.
    /// central reference for accessing textures
    /// </summary>
    public class SocketTexture
    {
        public static Texture open
        {
            get {  return Resources.Load("Textures/ShipEditor/socket_open", 
                                  typeof(Texture))as Texture; }
        }
        public static Texture closed
        {
            get { return Resources.Load("Textures/ShipEditor/socket_closed", 
                                      typeof(Texture))as Texture; }
        }
        public static Texture pending
        {
            get { return Resources.Load("Textures/ShipEditor/socket_pending",
                                    typeof(Texture)) as Texture; }
        }
    }
}

