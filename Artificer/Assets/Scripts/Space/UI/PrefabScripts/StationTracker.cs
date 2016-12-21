using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using Space.Segment;


namespace Space.UI.Ship
{
    public class StationTracker : MonoBehaviour
    {
        private StationController _station;

        public Slider Slider;

        public Text Label;

        public void DefineStation(StationController Station)
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

