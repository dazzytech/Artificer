using UnityEngine;
using System.Collections;

namespace Editor.Components
{
    public class ComponentSocket: ComponentObject
    {

        // Drawing socket piece
        protected Texture2D compulsary;

        // numbering sockets
        public int ID;

        public ComponentSocket()
        {
            compulsary = Resources.Load("Textures/ShipEditor/socket_open", 
                                  typeof(Texture2D))as Texture2D;
        }

        // Use this for initialization
        public override void Draw(Vector2 center)
        {
            Vector2 pos = Position + center;
            Rect sRect = new Rect(pos-new Vector2(4,4), new Vector2(8,8));
            GUI.DrawTexture (sRect, compulsary);
        }
    	
        public int dir
        {
            get
            {
                switch(socketDirection)
                {
                    case "up":
                        return 0;
                    case "down":
                        return 1;
                    case "left":
                        return 2;
                    case "right":
                        return 3;
                }
                return 0;
            }
        }
    }
}

