using UnityEngine;
using System.Collections;

namespace ArtificerEditor.Components
{
    public class FirePoint : ComponentObject
    {
        // Drawing socket piece
        protected Texture2D point;
        
        public FirePoint()
        {
            point = Resources.Load("Textures/ShipEditor/socket_closed", 
                                        typeof(Texture2D))as Texture2D;
            
        }
        public override void Draw(Vector2 center)
        {
            Vector2 pos = Position + center;
            Rect sRect = new Rect(pos-new Vector2(4,4), new Vector2(8,8));
            GUI.DrawTexture (sRect, point);
        }
    }
}

