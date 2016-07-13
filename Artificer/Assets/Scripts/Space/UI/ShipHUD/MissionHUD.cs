using UnityEngine;
using UnityEngine.UI;
using System.Collections;
// Artificer
using Data.Shared;

namespace Space.UI.Ship
{
    public class MissionHUD : MonoBehaviour
    {
        ContractData _contract;

        // HUD Elements
        public Transform _MissionHUD;
        public Transform _MissionList;

        // prefabs
        public GameObject _MissionPrefab;
        public GameObject _MissionDividerPrefab;
        public GameObject _ContractPrefab;

        // target
        public Transform _targetHUD;
        public GameObject StationPrefab;
        public GameObject DestroyPrefab;
        public GameObject TargetPrefab;

        // show/hide
        private bool _keyDelay = false;

        public void SetContactData(ContractData contract)
        {
            _contract = contract;
            BuildMissionList();
            SetTargetHUD();
        }

        private void BuildMissionList()
        {
            /*GameObject primaryDivider = Instantiate(_MissionDividerPrefab);
            primaryDivider.transform.SetParent(_MissionList, false);
            primaryDivider.transform.Find("Label").GetComponent<Text>().text = "Primary";

            if (_contract.PrimaryMissions != null)
            {
                foreach (MissionData mission in _contract.PrimaryMissions)
                {
                    GameObject newMission = Instantiate(_MissionPrefab);
                    newMission.transform.SetParent(_MissionList, false);

                    newMission.GetComponent<MissionHUDItem>().SetMission(mission);
                }
            }

            GameObject secondaryDivider = Instantiate(_MissionDividerPrefab);
            secondaryDivider.transform.SetParent(_MissionList, false);
            secondaryDivider.transform.Find("Label").GetComponent<Text>().text = "Optional";

            if (_contract.OptionalMissions != null)
            {
                foreach (MissionData mission in _contract.OptionalMissions)
                {
                    GameObject newMission = Instantiate(_MissionPrefab);
                    newMission.transform.SetParent(_MissionList, false);
                    
                    newMission.GetComponent<MissionHUDItem>().SetMission(mission);
                }
            }*/
        }

        private void SetTargetHUD()
        {
            /*foreach (MissionData m in _contract.PrimaryMissions)
            {
                if (m is DefendMission)
                {
                    GameObject sTracker = Instantiate(StationPrefab);
                    sTracker.transform.SetParent(_targetHUD, false);
                    sTracker.GetComponent<StationTracker>().DefineStation(((DefendMission)m).Station);
                    break;
                }

                if (m is AttritionMission)
                {
                    GameObject dTracker = Instantiate(DestroyPrefab);
                    dTracker.transform.SetParent(_targetHUD, false);
                    dTracker.GetComponent<DestroyTracker>().DefineMission((AttritionMission)m);
                    break;
                }

                if(m is TargetMission)
                {
                    GameObject mTracker = Instantiate(TargetPrefab);
                    mTracker.transform.SetParent(_targetHUD, false);
                    mTracker.GetComponent<TargetTracker>().DefineTargets((TargetMission)m);
                    break;
                }
            }*/
        }

        public void PauseRelease()
        {
            _keyDelay = false;
        }

        public void ToggleHUD()
        {
            if (!_keyDelay)
            {
                _MissionHUD.gameObject.SetActive
                    (!_MissionHUD.gameObject.activeSelf);
                _keyDelay = true;
                Invoke("PauseRelease", 0.3f);
            }
        }

    }
}
