using UnityEngine;
using System.Collections;

using Space.Ship.Components.Attributes;
using UnityEngine.Networking;

namespace Space.Ship.Components
{
    /// <summary>
    /// Component style utility.
    /// a shorthand way for access to the ship's style modifiers and accessors
    /// </summary>
    [RequireComponent(typeof(ComponentAttributes))]
    public class ComponentStyleUtility : NetworkBehaviour
    {
        private ComponentAttributes _att;

        #region ONSTART OVERRIDES

        /// <summary>
        /// Apply the ship style if it 
        /// was build on server already
        /// </summary>
        public override void OnStartClient()
        {
            _att = transform.GetComponent<ComponentAttributes>();

            if (_att.HasSpawned)
            {
                ApplyStyle();
            }
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Sets the style of the ship component.
        /// </summary>
        /// <param name="style">Style.</param>
        [Server]
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

        #endregion

        #region PRIVATE UTILITIES

        /// <summary>
        /// Applies the style to the component by swaping out the texture
        /// with the renderer.
        /// </summary>
        private void ApplyStyle()
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

        #endregion
    }
}

