using Data.UI;
using System.Collections;
using System.Collections.Generic;
using UI;
using UI.Effects;
using UnityEngine;
using UnityEngine.UI;

namespace Space.UI.Prompt
{
    /// <summary>
    /// Dynamic modular message system used to
    /// display prompts
    /// </summary>
    public class MessagePromptHUD : HUDPanel
    {
        #region ATTRIUBTES

        #region UI PREFABS

        [Header("Prefabs")]
        [SerializeField]
        private GameObject m_labelPrefab;

        [SerializeField]
        private GameObject m_sliderPrefab;

        #endregion 

        /// <summary>
        /// keeps track of each message for future reference
        /// </summary>
        private IndexedList<PromptData> m_promptLib;

        #endregion

        #region MONOBEHAVIOUR

        private void Awake()
        {
            m_promptLib = new IndexedList<PromptData>();
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Stores a prompt message and displays it 
        /// </summary>
        /// <returns>index of prompt</returns>
        public void DisplayPrompt(PromptData message)
        {
            // Quick check to ensure we aren't duplicating prompts
            int index = m_promptLib.IndexOf(message);
            if (index != -1)
            {
                index = m_promptLib[index].ID;
                // we have an existing, 
                // only build if hidden
                if (m_promptLib[index].UIBars == null &&
                    m_promptLib[index].UILabels == null)
                    CreatePromptUI(message);
            }
            else
            {
                // object is not stored
                // add to our list and full
                m_promptLib.Add(message);
            }
        }

        /// <summary>
        /// If a prompt is hidden we can
        /// pass its index here to reenable
        /// </summary>
        /// <param name="index"></param>
        public void DisplayPrompt(int index)
        {
            PromptData prompt = m_promptLib.Item(index);
            if (prompt != null)
                CreatePromptUI(prompt);
        }

        /// <summary>
        /// Clears the visual prompt but
        /// does not delete the prompt
        /// </summary>
        /// <param name="index"></param>
        public void HidePrompt(int index)
        {
            if (m_promptLib != null)
            {
                PromptData prompt = m_promptLib.Item(index);
                if (prompt != null)
                {
                    ClearPrompt(prompt);
                }
            }
        }

        /// <summary>
        /// Deletes and nullifys the prompt
        /// </summary>
        /// <param name="index"></param>
        public void DeletePrompt(int index)
        {
            if (m_promptLib != null)
            {
                PromptData prompt = m_promptLib.Item(index);
                if (prompt != null)
                {
                    ClearPrompt(prompt);
                    m_promptLib.Remove(prompt);
                    prompt = null;
                }
            }
        }

        #endregion

        #region PRIVATE UTILITIES

        /// <summary>
        /// Clears and nulls all the prompts ui elements
        /// </summary>
        /// <param name="prompt"></param>
        private void ClearPrompt(PromptData prompt)
        {
            if (prompt.UILabels != null)
            {
                // found prompt to disable
                foreach (Text ui in prompt.UILabels)
                {
                    PanelFadeEffects.FadeOutText(ui);
                    GameObject.Destroy(ui.gameObject);
                }

                prompt.UILabels = null;
            }

            if (prompt.UIBars != null)
            {
                // found prompt to disable
                foreach (HUDBar ui in prompt.UIBars)
                {
                    //PanelFadeEffects.FadeOutText(ui);
                    GameObject.Destroy(ui.gameObject);
                }

                prompt.UIBars = null;
            }
        }

        /// <summary>
        /// Creates the ui using prefabs and saves them
        /// </summary>
        /// <param name="prompt"></param>
        private void CreatePromptUI(PromptData prompt)
        {
            //PanelFadeEffects.FadeInText(PromptText);
        }

        #endregion

    }
}
