using UnityEngine;
using System.Collections;

/// <summary>
/// Component object.
/// Base object contained within a component
/// </summary>
namespace Editor.Components
{
    public class ComponentObject : ScriptableObject
    {
        public Vector2 Position;
        public string socketDirection;               
        public int socketType;                      // compulsary, optional, component
        /* 
         * 1 - compulsary
         * 2 - optional
         * 3 - component
         * 4 - hidden
         * */


        public virtual void Draw(Vector2 center)
        {
        }
    }
}

