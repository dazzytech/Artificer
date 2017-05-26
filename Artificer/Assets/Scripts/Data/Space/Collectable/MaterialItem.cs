using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data.Space.Collectable
{
    /// <summary>
    /// Object that when refined will yield 
    /// element items e.g asteroidpiece
    /// </summary>
    public class MaterialItem : ItemData
    {
        // change to inc yeild chance and yeild amount
        public string[] Composition;         // elements that item is made of
    }
}