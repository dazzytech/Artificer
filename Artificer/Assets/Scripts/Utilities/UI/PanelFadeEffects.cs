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
        /*public static void AnimInPanel(HUDPanel panel)
        {
            panel.DeactivateHUD();

            float origHeight, origWidth;

            RectTransform rect = panel.GetComponent<RectTransform>();
        }*/

        /// <summary>
        /// gives the appearance of an image
        /// flashing into existance (HUD item or panel)
        /// </summary>
        /// <param name="item"></param>
        public static void FlashInItem(RawImage item)
        {
            Color originalColor = item.color; 
            item.StartCoroutine(FadeImg(item,
                Color.white, 
                FadeImg(item, originalColor)));
        }

        #endregion

        #region COROUTINE

        /// <summary>
        /// Blends a colour with a pending Fade
        /// </summary>
        /// <param name="img"></param>
        /// <param name="goalColor"></param>
        /// <param name="NextFade"></param>
        /// <returns></returns>
        public static IEnumerator FadeImg(RawImage img, 
            Color goalColor, IEnumerator NextFade = null)
        {
            // Store the original color for tweening back
            Color currentColor = img.color;

            while (img.color != goalColor)
            {
                currentColor.r += BlendColourVal(currentColor.r, goalColor.r);
                currentColor.g += BlendColourVal(currentColor.g, goalColor.g);
                currentColor.b += BlendColourVal(currentColor.b, goalColor.b);
                currentColor.a += BlendColourVal(currentColor.a, goalColor.a);

                img.color = currentColor;

                yield return new WaitForSeconds(0.033f); // 30fps
            }

            if (NextFade != null)
                img.StartCoroutine(NextFade);

            yield break;
        }

        #endregion

        #region PRIVATE UTILITIES

        /// <summary>
        /// Returns the next step in the fade between two colours
        /// </summary>
        /// <param name="colFrom"></param>
        /// <param name="colTo"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        private static float BlendColourVal(float colFrom, float colTo, float step = 0.2f)
        {
            float result = colTo - colFrom;
            return colFrom != colTo ? Mathf.Abs(result) < step ? result :
                Mathf.Sign(result) * step : 0f;
        }

        #endregion
    }
}