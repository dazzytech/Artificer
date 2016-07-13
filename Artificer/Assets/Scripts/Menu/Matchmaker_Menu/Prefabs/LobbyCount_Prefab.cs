using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Menu
{
    public class LobbyCount_Prefab : MonoBehaviour 
    {
        public Text PlayerCounter;

        public void SetCounter(int count, int max)
        {
            PlayerCounter.text = string.Format("{0} -- {1}", count, max > 0? max.ToString(): "-");
        }
    }
}
