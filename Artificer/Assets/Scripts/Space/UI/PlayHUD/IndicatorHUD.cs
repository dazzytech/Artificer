using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using UI.Effects;

namespace Space.UI.Ship
{
    /// <summary>
    /// Displays nuremic data on HUD alerting
    /// player to changes e.g. Component Damage
    /// </summary>
    public class IndicatorHUD : HUDPanel
    {
        #region ATTRIBUTES

        private List<RectTransform> m_rects;

        #region FONTS

        [Header("Fonts")]

        [SerializeField]
        private Font m_damageComponentFont;
        
        [SerializeField]
        private Font m_healComponentFont;

        #endregion

        #region COLOR

        [Header("Colours")]

        [SerializeField]
        private Color m_damageComponentColour;
        
        [SerializeField]
        private Color m_healComponentColour;

        #endregion

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Create a text item at the specified
        /// location displaying the amount of integrity change
        /// </summary>
        /// <param name="location"></param>
        /// <param name="amount"></param>
        public void IndicateIntegrity
            (Vector2 location, float amount)
        { 
            // Build the game object 
            GameObject IndicationText = new GameObject();

            RectTransform rect = IndicationText.
                AddComponent<RectTransform>();

            rect.sizeDelta = new Vector2(45, 20);

            // set parent while preseving postion
            IndicationText.transform.SetParent(m_HUD.transform, false);

            // convert from world to camera pos
            IndicationText.transform.localPosition = UIConvert.
                WorldToCameraPoint(location);
                
            Overlap(rect);

            // build Text object
            Text textItem = IndicationText.AddComponent<Text>();

            // set font on sign of amount
            textItem.font = amount > 0 ? m_healComponentFont : m_damageComponentFont;
            textItem.color = amount > 0 ? m_healComponentColour : m_damageComponentColour;
            textItem.fontSize = 10;
            textItem.alignment = TextAnchor.MiddleCenter;

            // set text (always positive number)
            textItem.text = Mathf.Abs(amount).ToString("F0");

            // Fade in the text
            PanelFadeEffects.FadeInText(textItem);

            // Destroy after delay
            Destroy(IndicationText, 1f);

            m_rects.Add(rect);
        }

        #endregion

        #region PRIVATE UTILITIES

        /// <summary>
        /// Shifts rect upwards if it is currently overlaying
        /// an item
        /// </summary>
        /// <param name="rect"></param>
        public void Overlap(RectTransform rect, int occurance = 0)
        {
            if (m_rects == null)
                m_rects = new List<RectTransform>();

            if(occurance >= 5)
            {
                Destroy(rect.gameObject);
                return;
            }

            for(int i = 0; i < m_rects.Count; i++)
            {
                RectTransform r = m_rects[i];

                if (r == null)
                {
                    m_rects.RemoveAt(i);
                    i--;
                    continue;
                }

                if (r == rect)
                    continue;

                Vector2 Distance = r.anchoredPosition - rect.anchoredPosition;

                if ( Mathf.Abs(Distance.x) < ((r.rect.width + rect.rect.width)/2)
                    && Mathf.Abs(Distance.y) < ((r.rect.height + rect.rect.height) / 2))
                {
                    r.anchoredPosition =
                        r.anchoredPosition +
                        (new Vector2(0, 21f));

                    Overlap(r, ++occurance);

                    return;
                }
            }
        }

        #endregion
    }
}
