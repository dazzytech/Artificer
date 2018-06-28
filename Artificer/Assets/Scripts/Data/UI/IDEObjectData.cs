using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data.UI
{
    public enum Alignment { SELF, FRIENDLY, NEUTRAL, ENEMY };

    /// <summary>
    /// Any variable type that is not a primative type
    /// must extend from this type for polymorphism
    /// </summary>
    public class IDEObjectData
    { }

    /// <summary>
    /// Describes information for an entity within space
    /// </summary>
    public class EntityObject: IDEObjectData
    {
        public Transform Reference;

        public Alignment Alignment;
    }
}
