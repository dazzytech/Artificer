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
        public void DisplayPrompt(PromptData message, float timer = 0)
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
                CreatePromptUI(message);
            }

            if(timer > 0)
            {
                StartCoroutine(ClearPromptDelay(message, timer));
            }
        }

        /// <summary>
        /// If a prompt is hidden we can
        /// pass its index here to reenable
        /// </summary>
        /// <param name="index"></param>
        public void DisplayPrompt(int index, float timer = 0)
        {
            PromptData prompt = m_promptLib.Item(index);
            if (prompt != null)
                CreatePromptUI(prompt);

            if (timer > 0)
            {
                StartCoroutine(ClearPromptDelay(prompt, timer));
            }
        }

        /// <summary>
        /// Quick means of 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="timer"></param>
        public void DisplayPrompt(string message, float timer = 0)
        {
            PromptData prompt = new PromptData()
                { LabelText = new string[1] { message } };

            if (prompt != null)
                CreatePromptUI(prompt);

            if (timer > 0)
            {
                StartCoroutine(ClearPromptDelay(prompt, timer));
            }
        }

        /// <summary>
        /// Takes the id of the prompt and 
        /// updates an UI elements accociated with 
        /// that prompt
        /// </summary>
        /// <param name="index"></param>
        public void UpdatePrompt(int index)
        {
            if (m_promptLib != null)
            {
                PromptData prompt = m_promptLib.Item(index);
                if (prompt != null)
                {
                    if (prompt.LabelText != null)
                    {
                        if (prompt.UILabels == null)
                            prompt.UILabels = new Text[1];

                        // we are gonna resize the current store list 
                        // to an updated size if needed
                        List<Text> temp = new List<Text>(prompt.UILabels);

                        int i = 0;
                        // found prompt to disable
                        foreach (string text in prompt.LabelText)
                        {
                            if (i > temp.Count)
                                temp.Add(CreateText(text));
                            else
                            {
                                if (temp[i] == null)
                                    temp[i] = CreateText(text);
                                else
                                    temp[i].text = text;
                            }
                        }

                        prompt.UILabels = temp.ToArray();
                    }

                    if (prompt.SliderValues != null)
                    {
                       
                        if (prompt.UIBars == null)
                            prompt.UIBars = new HUDBar[1];

                        // we are gonna resize the current store list 
                        // to an updated size if needed
                        List<HUDBar> temp = new List<HUDBar>(prompt.UIBars);

                        int i = 0;

                        // found prompt to disable
                        foreach (float value in prompt.SliderValues)
                        {
                            if (i > temp.Count)
                                temp.Add(CreateBar(value));
                            else
                            {
                                if (temp[i] == null)
                                    temp[i] = CreateBar(value);
                                else
                                    temp[i].Value = value;
                            }
                        }

                        prompt.UIBars = temp.ToArray();
                    }
                }
            }
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

        #region CLEAR UI

        /// <summary>
        /// Invokes the clear method after a delay
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        private IEnumerator ClearPromptDelay(PromptData prompt, float seconds)
        {
            yield return new WaitForSeconds(seconds);

            ClearPrompt(prompt);

            int index = m_promptLib.IndexOf(prompt);
            if (index == -1)
            {
                // some automated prompts wont be in list
                // and can be destroyed after clear
                prompt = null;
            }

            yield break;
        }

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
                    GameObject.Destroy(ui.gameObject, 1);
                }

                prompt.UILabels = null;
            }

            if (prompt.UIBars != null)
            {
                // found prompt to disable
                foreach (HUDBar ui in prompt.UIBars)
                {
                    //PanelFadeEffects.FadeOutText(ui);
                    GameObject.Destroy(ui.gameObject, 1);
                }

                prompt.UIBars = null;
            }
        }

        #endregion

        #region UI CREATION

        /// <summary>
        /// Creates the ui using prefabs and saves them
        /// </summary>
        /// <param name="prompt"></param>
        private void CreatePromptUI(PromptData prompt)
        {
            if (prompt.LabelText != null)
            {
                prompt.UILabels = new Text[prompt.LabelText.Length];
                int i = 0;
                foreach (string label in prompt.LabelText)
                {
                    prompt.UILabels[i++] = CreateText(label);
                }
            }

            if (prompt.SliderValues != null)
            {
                prompt.UIBars = new HUDBar[prompt.SliderValues.Length];
                int i = 0;
                foreach (float value in prompt.SliderValues)
                {
                    prompt.UIBars[i++] = CreateBar(value);
                }
            }
        }

        /// <summary>
        /// Creates and returns a text element
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        private Text CreateText(string label)
        {
            GameObject gameObject = Instantiate(m_labelPrefab);
            gameObject.transform.SetParent(m_HUD);
            
            Text text = gameObject.GetComponent<Text>();
            text.text = label;
            PanelFadeEffects.FadeInText(text);

            return text;
        }

        /// <summary>
        /// Creates and returns a HUD Bar element
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private HUDBar CreateBar(float value)
        {
            GameObject gameObject = Instantiate(m_sliderPrefab);
            gameObject.transform.SetParent(m_HUD);

            HUDBar bar = gameObject.GetComponent<HUDBar>();
            bar.SetColour(Color.grey);
            bar.Value = value;

            return bar;
        }

        #endregion

        #endregion

    }
}
