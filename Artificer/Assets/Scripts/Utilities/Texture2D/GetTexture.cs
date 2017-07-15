using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetTexture : MonoBehaviour
{
    /// <summary>
    /// Extracts the texture from
    /// the first sprite renderer
    /// within the GO's children
    /// </summary>
    /// <param name="GO"></param>
    /// <returns></returns>
    public static Texture2D Get(GameObject GO)
    {
        // if component has a renderer then
        // set the sprite as comp texture
        SpriteRenderer newTex = null;

        // retreive texture from the first child object
        foreach (Transform child in GO.transform)
        {
            newTex = child.GetComponent<SpriteRenderer>();
            if (newTex != null)
                break;
        }

        // return our new texture
        return newTex.sprite.texture;
    }
}
