using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Space.UI.Ship
{
    /// <summary>
    /// Uses collection of square sprites 
    /// to display station integrity
    /// </summary>
    public class IntegrityTracker : MonoBehaviour
    {
        #region ATTRIBUTES

        // store a list of the integrity bar segments
        [SerializeField]
        public Image[] ImageList;

        [SerializeField]
        private bool Recolor;

        private Color HighColor;

        [SerializeField]
        private Color MedColor;

        [SerializeField]
        private Color LowColor;

        #endregion

        void Awake()
        {
            HighColor = ImageList[0].color;
        }

        #region PUBLIC INTERACTION

        /// <summary>
        /// Displays and hides segments based on 
        /// normalvalue
        /// </summary>
        /// <param name="normalValue"></param>
        public void Step(float normalValue)
        {
            if(Recolor)
            {
                if (normalValue <= 1f)
                    foreach (Image segment in ImageList)
                        segment.color = HighColor;
                if (normalValue <= 0.6f)
                    foreach (Image segment in ImageList)
                        segment.color = MedColor;
                if (normalValue <= 0.3f)
                    foreach (Image segment in ImageList)
                        segment.color = LowColor;
            }

            // Simply go down in increments showing and hiding each segment
            for(int i = 0, a = ImageList.Length-1; i < ImageList.Length; i++, a--)
            {
                // 1.0 - (1 * 0.1f) = 0.9
                CompareValue(normalValue, (1.0f - (i+1) * 0.1f), ImageList[a]);
            }
        }

        #endregion

        #region PRIVATE FUNCTIONS

        /// <summary>
        /// reusing code which shows or hide image depending on normal value
        /// </summary>
        /// <param name="normalValue"></param>
        /// <param name="compare"></param>
        /// <param name="segment"></param>
        private void CompareValue(float normalValue, float compare, Image segment)
        {
            if (normalValue <= compare)
                segment.CrossFadeAlpha(0f, 0f, true);
            else
                segment.CrossFadeAlpha(1f, 0f, true);
        }

        #endregion
    }
}