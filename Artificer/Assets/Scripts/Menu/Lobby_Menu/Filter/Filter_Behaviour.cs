using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Steamworks;

namespace Menu
{
    public class FilterInfo
    {
        // Populated with all filters from window
        public string LobbyName;
        public int SlotsAvailable;
        public int MaxLobbies;
    }
    public class Filter_Behaviour : MonoBehaviour
    {
        // UI Elements
        [SerializeField]
        private InputField MaxLobbyCount;

        [SerializeField]
        private InputField LobbyName;

        [SerializeField]
        private InputField SlotCount;

        void Awake()
        {
            SlotCount.contentType = InputField.ContentType.IntegerNumber;
            SlotCount.characterLimit = 2;
            MaxLobbyCount.contentType = InputField.ContentType.IntegerNumber;
            MaxLobbyCount.characterLimit = 3;

            LobbyName.characterLimit = 50;
        }

        /// <summary>
        /// Retrives any text in non-empty input fields and applies them
        /// as a filter
        /// </summary>
        public FilterInfo ApplyFilter()
        {
            FilterInfo newFilter = new FilterInfo();

            // Copy from inputs to filter var
            if(LobbyName.text.Length > 0)
                newFilter.LobbyName = LobbyName.text;

            int.TryParse(SlotCount.text, out newFilter.SlotsAvailable);

            int.TryParse(MaxLobbyCount.text, out newFilter.MaxLobbies);

            return newFilter;
        }
    }
}

