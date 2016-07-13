using UnityEngine;
using UnityEngine.UI;
using System.Collections;
// Artificer

namespace Space.UI
{
    public class MissionHUDItem : MonoBehaviour {
        public Text Name;
        public Text Elapsed;
        public Text Descriptions;
        public Text Status;
        public Text Objectives;
    	
        //public MissionData Mission;

        void OnDestroy()
        {
            StopCoroutine("UpdateStats");
        }

        void OnEnable()
        {
            StartCoroutine("UpdateStats");
        }

        /*public void SetMission(MissionData data)
        {
            Mission = data;
        }*/

        IEnumerator UpdateStats()
        {
            while (true)
            {
                /*if (Mission != null)
                {
                    Name.text = Mission.Title;
                    if (Mission.TimeLimit != 0)
                        Elapsed.text = Mission.Remaining.ToString("F0") + " seconds remaining.";
                    Status.text = Mission.Success ? "Completed" : Mission.Failure ? "Failed" : "Pending";
                    Objectives.text = Mission.Objectives();
                }*/

                yield return null;
            }
        }
    }
}