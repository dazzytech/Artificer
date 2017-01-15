using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
// Artificer
using Data.Shared;
using Space.Ship.Components.Listener;
using Space.Ship.Components.Attributes;

namespace Construction.ShipEditor
{
    public class DrawObject
    {
        public Texture2D ShipPart;
        public Rect ShipBounds;
    }

    public class ShipTextureUtil
    {
        public void SaveIcon(List<BaseShipComponent> bscs, ShipData Ship)
        {
            // Create sprite atlas
            Texture2D atlas;
            List<DrawObject> objs = new List<DrawObject>();
            int left = int.MaxValue;
            int top = int.MaxValue;
            int width = 0;
            int height = 0;
            
            foreach (BaseShipComponent bsc in bscs)
            {
                Rect newBounds = new Rect();
                if (bsc.Bounds.xMin < left) left = (int)bsc.Bounds.xMin;
                if (bsc.Bounds.yMin < top) top = (int)bsc.Bounds.yMin;
                
                if (bsc.direction == "up" || bsc.direction == "down")
                {
                    newBounds = bsc.Bounds; 
                }
                else
                {
                    newBounds = new Rect(bsc.Bounds.x, bsc.Bounds.y,
                                         bsc.Bounds.height, bsc.Bounds.width);
                }
                
                if (newBounds.xMax > width) width = (int)newBounds.xMax;
                if (newBounds.yMax > height) height = (int)newBounds.yMax;
                
            }
            
            // Create texture items
            foreach (BaseShipComponent bsc in bscs)
            {
                Rect newBounds = new Rect();

                if (bsc.direction == "up" || bsc.direction == "down")
                {
                    newBounds = bsc.Bounds; 
                }
                else
                {
                    newBounds = new Rect(bsc.Bounds.x, bsc.Bounds.y,
                                         bsc.Bounds.height, bsc.Bounds.width);
                } 

                DrawObject obj = new DrawObject();
                
                // boundies
                obj.ShipBounds = new Rect();
                
                obj.ShipBounds.x = newBounds.x - left;
                obj.ShipBounds.y = newBounds.y - top;
                
                obj.ShipBounds.width = newBounds.width;
                obj.ShipBounds.height = newBounds.height;
                
                int origY = 0;
                int startY = 0;
                int dirY = 1;
                
                int origX = 0;
                int startX = (int)obj.ShipBounds.width;
                int dirX = -1;
                
                obj.ShipPart = new Texture2D((int)obj.ShipBounds.width, (int)obj.ShipBounds.height, 
                                             TextureFormat.RGBA32, false);
                
                if (bsc.direction == "up" || bsc.direction == "down")
                {
                    
                    if (bsc.direction == "down")
                    {
                        startY = (int)obj.ShipBounds.height;
                        dirY = -1;
                    }
                    
                    for (int y = startY; origY < bsc.Bounds.height; origY++)
                    {
                        for (int x = 0; x < obj.ShipBounds.width; x++)
                        {
                            
                            obj.ShipPart.SetPixel(x, y, 
                                bsc.GetTex.GetPixel(x, origY));
                        }
                        y += dirY;
                    }
                }
                else
                {
                    if (bsc.direction == "right")
                    {
                        startX = 0;
                        dirX = 1;
                    }
                    
                    for (int x = startX; origY < bsc.Bounds.height; origY++)
                    {
                        for (int y = startY; origX < bsc.Bounds.width; origX++)
                        {
                            obj.ShipPart.SetPixel(x, y,
                                bsc.GetTex.GetPixel(origX, origY));
                            y += dirY;
                        }
                        x += dirX;
                        origX = 0;
                    }
                }
                
                obj.ShipPart.Apply();
                objs.Add(obj);
            }
            
            width -= left;
            height -= top;
            
            // create new texture
            atlas = new Texture2D(width,height, TextureFormat.RGBA32, false);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool coloured = false;
                    foreach (DrawObject obj in objs)
                    {
                        if(obj.ShipBounds.Contains(new Vector2(x, y)))
                        {
                            if(obj.ShipPart.GetPixel
                               (x - (int)obj.ShipBounds.x,
                             y - (int)obj.ShipBounds.y).a != 0)
                            {
                                atlas.SetPixel(x, y, obj.ShipPart.GetPixel
                                               (x - (int)obj.ShipBounds.x, y - (int)obj.ShipBounds.y));

                                coloured = true;
                            }
                        }
                    }
                    if(!coloured)
                        atlas.SetPixel(x, y, new Color(0f, 0f, 0f, 0f));
                }
            }
            
            atlas.Apply();
            string path = (System.Environment.CurrentDirectory) + "/Space/icons";
            Directory.CreateDirectory(path);
            File.WriteAllBytes (path + "/" + Ship.Name + ".png", atlas.EncodeToPNG ());
            //Ship.IconTex = atlas;
        }

        // need to add a delete function
        public void Delete(string shipName)
        {
            string path = (System.Environment.CurrentDirectory) + "/Space/icons";
            if (File.Exists(path + "/" + shipName + ".png"))
            {
                File.Delete(path + "/" + shipName + ".png");
            }
        }
    }
}
