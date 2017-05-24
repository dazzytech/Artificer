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
        public int[] Yield;         // item ID of elements returned when refined
    }
}