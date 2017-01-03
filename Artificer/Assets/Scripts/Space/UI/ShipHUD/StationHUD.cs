using UnityEngine;
using System.Collections;

//Artificer

using Space.Segment;

namespace Space.UI.Ship
{
    /// <summary>
    /// Resposible for adding station information
    /// to the HUD
    /// </summary>
    public class StationHUD : MonoBehaviour
    {
        #region ATTRIBUTES

        private bool _keyDelay = false;

        #region HUD ELEMENTS

        public Transform HUD;
        public Transform StationList;

        #endregion

        #region PREFABS

        public GameObject StationPrefab;

        #endregion

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Adds station to the HUD display
        /// </summary>
        /// <param name="piece"></param>
        public void AddUIPiece(StationController controller)
        {
            GameObject newStation = Instantiate(StationPrefab);
            newStation.transform.SetParent(StationList);

            StationTracker tracker = newStation.GetComponent<StationTracker>();
            tracker.DefineStation(controller);
        }

        public void ToggleHUD()
        {
            if (!_keyDelay)
            {
                HUD.gameObject.SetActive
                    (!HUD.gameObject.activeSelf);
                _keyDelay = true;
                Invoke("PauseRelease", 0.3f);
            }
        }

        #endregion

        #region COROUTINES

        public void PauseRelease()
        {
            _keyDelay = false;
        }

        #endregion
    }
}