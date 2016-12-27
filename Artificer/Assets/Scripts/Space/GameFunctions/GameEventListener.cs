using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

using Space.Ship;

namespace Space.GameFunctions
{
    public class GameEventListener : NetworkBehaviour
    {
        #region ATTRIBUTES

        private GameAttributes _att;

        #endregion

        #region MONO BEHAVIOUR

        void OnEnable()
        {
            // Assign Events
            ShipMessageController.OnShipDestroyed += ProcessShipDestroyed;
        }

        void OnDisable()
        {
            // De-assign events
            ShipMessageController.OnShipDestroyed -= ProcessShipDestroyed;
        }

        // Use this for initialization
        void Awake()
        {
            _att = GetComponent<GameAttributes>();
        }

        #endregion

        #region EVENT LISTENERS

        public void ProcessShipDestroyed(DestroyDespatch destroyed)
        {
            /*foreach (MissionData mission in PrimaryTracker)
            {
                mission.AddShipKilled(destroyed);
            }
            foreach (MissionData mission in SecondaryTracker)
            {
                mission.AddShipKilled(destroyed);
            }*/
        }

        public void ProcessStationDestroyed(DestroyDespatch destroyed)
        {

        }

        /*
        public void ProcessStationReached(Transform ship)
        {
            foreach (MissionData mission in PrimaryTracker)
            {
                mission.StationEntered(ship);
            }
            foreach (MissionData mission in SecondaryTracker)
            {
                mission.StationEntered(ship);
            }
        }

        public void ProcessMaterials(Dictionary<MaterialData, float> newMat)
        {
            foreach (MissionData mission in PrimaryTracker)
            {
                mission.AddMaterial(newMat);
            }
            foreach (MissionData mission in SecondaryTracker)
            {
                mission.AddMaterial(newMat);
            }
        }*/

        #endregion
    }
}
