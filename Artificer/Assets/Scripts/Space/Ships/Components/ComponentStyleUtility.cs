using UnityEngine;
using System.Collections;

namespace ShipComponents
{
    /// <summary>
    /// Component style utility.
    /// a shorthand way for access to the ship's style modifiers and accessors
    /// </summary>
    [RequireComponent(typeof(ComponentAttributes))]
    public class ComponentStyleUtility : MonoBehaviour
    {
        ComponentAttributes _att;

        // Use to add the current style to the component
        void Start()
        {
            _att = transform.GetComponent<ComponentAttributes>();
        }
    	
        /// <summary>
        /// Sets the style of the ship component.
        /// </summary>
        /// <param name="style">Style.</param>
        public void SetStyle(string style)
        {
            if(_att == null)
                _att = transform.GetComponent<ComponentAttributes>();

            if (_att.currentStyle == style)
                return;

            foreach (StyleInfo info in _att.componentStyles)
            {
                if (info.name == style)
                    _att.currentStyle = style;
            }
            // possibly call function to apply texture
            ApplyStyle();
        }

        /// <summary>
        /// Gets the current style as a texture 2D.
        /// </summary>
        /// <returns>The style.</returns>
        public Sprite GetStyle()
        {
            foreach (StyleInfo info in _att.componentStyles)
            {
                if (info.name == _att.currentStyle)
                    return info.sprite;
            }
            return null;
        }

        /// <summary>
        /// Applies the style to the component by swaping out the texture
        /// with the renderer.
        /// </summary>
        public void ApplyStyle()
        {
            SpriteRenderer render = GetComponentInChildren<SpriteRenderer>();

            if (render == null)
            {
                print("Error - Component Style Utility: cannot access renderer!");
                return;
            }

            Sprite newStyle = GetStyle();
            if(newStyle == null)
            {
                print("Error - Component Style Utility: style does not exist!");
                return;
            }

            // swap texture if not currently the same
            if (render.sprite == newStyle)
                return;
            else
                render.sprite = newStyle;
        }
    }
}

