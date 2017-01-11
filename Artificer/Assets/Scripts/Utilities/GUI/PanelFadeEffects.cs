using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Timers;

namespace UI.Effects
{
    public class PanelFadeEffects
    {
        #region PUBLIC INTERACTION

        /// <summary>
        /// Makes the text appear to fade in using the alpha 
        /// control
        /// </summary>
        /// <param name="Panel"></param>
        public static void FadeInText(Text FadeText)
        {
            FadeText.canvasRenderer.SetAlpha(0f);

            FadeText.CrossFadeAlpha(1.0f, .1f, false);
        }

        public static void FadeOutText(Text FadeText)
        {
            FadeText.CrossFadeAlpha(0f, .1f, false);
        }

        /// <summary>
        /// Create the appearance of the panel 
        /// Spreading across the screen with an initial white
        /// </summary>
        /// <param name="panel"></param>
        public static void AnimInPanel(Space.UI.BasePanel panel)
        {
            panel.DeactivateHUD();

            float origHeight, origWidth;

            RectTransform rect = panel.GetComponent<RectTransform>();


        }

        #endregion

        #region PRIVATE UTILITIES

        #endregion
    }
}