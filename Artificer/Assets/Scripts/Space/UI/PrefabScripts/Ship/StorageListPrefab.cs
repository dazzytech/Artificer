using System.Collections;
using System.Collections.Generic;
using UI;
using UI.Effects;
using UnityEngine;
using UnityEngine.UI;

namespace Space.UI
{
    /// <summary>
    /// Used to display the amount of a material 
    /// stored in the ship
    /// </summary>
    public class StorageListPrefab : SelectableHUDItem
    {
        #region ATTRIBUTES

        [Header("Storage")]

        /// <summary>
        /// displays the name
        /// </summary>
        [Header("HUD Elements")]
        [SerializeField]
        private Text m_label;

        /// <summary>
        /// Retrieve rect transform
        /// because we rely on scale
        /// </summary>
        [SerializeField]
        private RectTransform m_bar;

        /// <summary>
        /// Show the total amount faded
        /// </summary>
        [SerializeField]
        private RectTransform m_total;

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Visually updates bar to display 
        /// storage value
        /// value 0f - 1f
        /// </summary>
        /// <param name="value"></param>
        /// <param name="name"></param>
        public void DisplayValue(float value, string name, float total)
        {
            StopAllCoroutines();

            // display text
            m_label.text = name;

            if(Mathf.Abs(value - m_bar.localScale.x) > 0.005f)
            {
                // resize the bar to the new value
                StartCoroutine(LerpBar(m_bar, value));

                FlashImage();
            }

            if(Mathf.Abs(total - m_total.localScale.x) > 0.005f)
            {
                // resize the bar to the new value
                StartCoroutine(LerpBar(m_total, total));
            }
        }

        #endregion

        #region PRIVATE UTILITIES

        /// <summary>
        /// gradually scales bar from current value to 
        /// new value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private IEnumerator LerpBar(RectTransform bar, float value)
        {
            while(bar.localScale.x != value)
            {
                bar.localScale = new Vector3
                    (Mathf.Lerp(bar.localScale.x, value, 0.1f), 1f, 1f);

                yield return new WaitForSeconds(0.033f); // 30fps
            }

            yield break;
        }

        #endregion
    }
}
