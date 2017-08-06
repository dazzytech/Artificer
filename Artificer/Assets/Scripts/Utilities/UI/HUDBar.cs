using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Contains a value and displays a bar
    /// at that value
    /// </summary>
    public class HUDBar : MonoBehaviour
    {
        #region ATTRIBUTES

        /// <summary>
        /// CLAMPED - Bar instantly displays value
        /// FLUID - Bar lerps to value
        /// </summary>
        public enum BarStyle { CLAMPED, FLUID };

        private float m_value;

        [SerializeField]
        private BarStyle m_style;

        [SerializeField]
        private Color m_colour = Color.white;

        #region HUD ELEMENTS

        [SerializeField]
        private RectTransform m_bar;

        #endregion

        #endregion

        #region MONO BEHAVIOUR

        private void OnEnable()
        {
            m_bar.GetComponent<RawImage>().color
                = m_colour;

            Value = 0;
        }

        #endregion

        #region ACCESSOR

        /// <summary>
        /// Value for the bar to display
        /// between 0f & 1f
        /// </summary>
        public float Value
        {
            set
            {
                // Assign and clamp the value
                m_value = value;
                m_value = Mathf.Clamp(m_value, 0f, 1f);

                // Display value based on style
                switch(m_style)
                {
                    case BarStyle.CLAMPED:
                        m_bar.localScale = new Vector3
                            (value, 1f, 1f);
                        break;
                    case BarStyle.FLUID:
                        StopAllCoroutines();
                        StartCoroutine("LerpBar", value);
                        break;
                }

            }
            get
            {
                return m_value;
            }
        }

        /// <summary>
        /// Update the colour of the bar
        /// </summary>
        public Color Colour
        {
            set
            {
                m_colour = value;
                m_bar.GetComponent<RawImage>().color
                    = m_colour;
            }
        }

        #endregion

        #region PRIVATE UTITLITIES

        /// <summary>
        /// gradually scales bar from current value to 
        /// new value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private IEnumerator LerpBar(float value)
        {
            while (m_bar.localScale.x != value)
            {
                m_bar.localScale = new Vector3
                    (Mathf.Lerp(m_bar.localScale.x, value, 
                    Mathf.Min(1.0f, Mathf.Abs(m_bar.localScale.x-value))), 1f, 1f);

                yield return new WaitForSeconds(0.033f); // 30fps
            }

            yield break;
        }

        #endregion
    }
}
