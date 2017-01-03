using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

//Artificer
using Space.Segment;

namespace Space.UI.Ship
{
    /// <summary>
    /// Local HUD element that tracks state of station
    /// </summary>
    public class StationTracker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        #region ATTRIBUTES

        private StationController _station;

        public IntegrityTracker Integrity;

        #region HUD ELEMENTS

        public Text Label;

        public Text Distance;

        public Text Status;

        public Image Icon;

        public Image SelfPanel;

        #endregion

        #region COLORS

        public Color safeColor;

        public Color attackColor;

        public Color destroyedColor;

        public Color highlightColor;

        private Color standardColor;

        #endregion

        #endregion

        public void DefineStation(StationController Station)
        {
            _station = Station;

            Label.text = Station.name;

            Icon.overrideSprite = _station.Icon;

            standardColor = SelfPanel.color;

            StartCoroutine("Step");
        }

        #region COROUTINE

        private IEnumerator Step()
        {
            while(true)
            {
                if (_station == null)
                {
                    Destroy(this.gameObject);
                    StopAllCoroutines();
                    break;
                }

                // Pass info to integrity tracker 
                Integrity.Step(_station.NormalizedHealth);

                // Update station status
                switch(_station.Status)
                {
                    // change and recolor text based on station state
                    case 0:
                        Status.text = "Safe";
                        Status.color = safeColor;
                        break;

                    case 1:
                        Status.text = "Attacked";
                        Status.color = attackColor;
                        break;

                    case 2:
                        Status.text = "Destroyed";
                        Status.color = destroyedColor;
                        break;
                }

                // Find distance from station to player

                // Retrieve player object and check if 
                // Player object currently exists
                float distance = _station.Distance;

                if(distance == -1)
                {
                    Distance.text = "-";
                    yield return null;
                }

                // display distance
                Distance.text = ((int)distance / 10).ToString() + "km";

                //finished this step
                yield return null;
            }

            yield return null;
        }

        #endregion

        #region IPOINTEREVENTS

        public void OnPointerEnter(PointerEventData eventData)
        {
            SelfPanel.color = highlightColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SelfPanel.color = standardColor;
        }

        #endregion
    }
}

