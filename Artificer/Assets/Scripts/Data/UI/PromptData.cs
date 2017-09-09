using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UI;

namespace Data.UI
{
    /// <summary>
    /// Contains object stored within the
    /// prompt and prompt HUD displays and stores it
    /// </summary>
    public class PromptData : IndexedObject
    {
        /// <summary>
        /// Each label will spawn a label object and is 1st
        /// </summary>
        public string[] LabelText;
        /// <summary>
        /// Each slider val creates a slider prefab
        /// </summary>
        public float[] SliderValues;
        /// <summary>
        /// Text Elements that have been created for
        /// this prompt
        /// </summary>
        public Text[] UILabels;
        /// <summary>
        /// Slider Elements that have been created for
        /// this prompt
        /// </summary>
        public HUDBar[] UIBars;
    }
}