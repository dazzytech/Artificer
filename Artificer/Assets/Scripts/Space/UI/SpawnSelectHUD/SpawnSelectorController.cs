using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Timers;

namespace Space.UI.Spawn
{
    /// <summary>
    /// Controller for spawn selector
    /// responsible for making changes to the UI
    /// </summary>
    public class SpawnSelectorController : MonoBehaviour
    {
        #region UI ELEMENTS

        public Button SpawnButton;
        public Text SpawnDelayText;

        #endregion

        #region ATTRIBUTES

        private SpawnSelectorEventListener _listener;
        private Timer _spawnerTimer;
        private float _delay;

        #endregion

        #region MONO BEHAVIOUR

        void Awake()
        {
            // disable Spawn UI Elements
            SpawnButton.interactable = false;

            _listener = GetComponent<SpawnSelectorEventListener>();

            // Initialize timer
            _spawnerTimer = new Timer(1000);
        }

        void Update()
        {
            if (_delay > 0)
                SpawnDelayText.text = string.Format("Ready to spawn in: {0} sec.", _delay);
            else
            {
                SpawnDelayText.text = "Ready to spawn.";
                if(!SpawnButton.interactable)
                    SpawnButton.interactable = true;
            }
        }

        #endregion

        #region SPAWN FUNCTIONALITY

        /// <summary>
        /// Called by event listener to start the spawn delay countdown
        /// </summary>
        public void EnableSpawn(float delay)
        {
            _delay = delay;

            _spawnerTimer.Elapsed += OnTimedEvent;
            _spawnerTimer.Start();
        }

        public void OnTimedEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            _delay--;

            if (_delay <= 0)
            {
                _spawnerTimer.Stop();
                _spawnerTimer.Elapsed -= OnTimedEvent;
                _spawnerTimer.Close();
            }
        }

        #endregion
    }
}