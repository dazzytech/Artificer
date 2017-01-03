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
        public Image[] ImageList = new Image[10];

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Displays and hides segments based on 
        /// normalvalue
        /// </summary>
        /// <param name="normalValue"></param>
        public void Step(float normalValue)
        {
            // Simply go down in increments showing and hiding each segment
            for(int i = 0; i < 10;)
            {
                // 1.0 - (1 * 0.1f) = 0.9
                CompareValue(normalValue, (1.0f - ++i * 0.1f), ImageList[i]);
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
                segment.enabled = false;
            else
                segment.enabled = true;
        }

        #endregion
    }
}