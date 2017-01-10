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
            // Create timer to update at 30fps
            // and pass text as parameter
            //Timer internalClock = new Timer(32);
            //internalClock.Elapsed +=
             //   delegate { fadeIn_Elapsed(internalClock, FadeText); };

            // auto reset and repeat elapsed event
            //internalClock.AutoReset = true;

            // Set colour in text to 0
            //FadeText.color = new Color(FadeText.color.r, FadeText.color.g,
                //FadeText.color.b, 0f);

            FadeText.canvasRenderer.SetAlpha(0f);

            FadeText.CrossFadeAlpha(1.0f, .1f, false);

            // start timer
            //internalClock.Enabled = true;
        }

        #endregion

        #region PRIVATE UTILITIES

        /* private static void fadeIn_Elapsed(Timer internalClock, Text FadeText)
         {
             // Create new color for the next text fade step
             Color newColor = new Color(FadeText.color.r, FadeText.color.g,
                 FadeText.color.b, FadeText.color.a + (float)internalClock.Interval*0.01f);

             FadeText.col
         }
         */
        #endregion
    }
}