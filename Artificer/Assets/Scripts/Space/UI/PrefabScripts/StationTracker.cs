using UnityEngine;
using UnityEngine.UI;
using System.Collections;


namespace Space.UI.Ship
{
    public class StationTracker : MonoBehaviour
    {
        private StationBehaviour _station;

        public Slider Slider;

        public Text Label;

        public void DefineStation(StationBehaviour Station)
        {
            Label.text = Station.name;

            _station = Station;
        }

        void Update()
        {
            if (_station != null)
            {
                //Slider.value = _station.NormalizedHealth;
            }
        }
    }
}

