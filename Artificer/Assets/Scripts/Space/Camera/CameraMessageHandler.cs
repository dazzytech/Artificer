using UnityEngine;
using System.Collections;

namespace Space.CameraUtils
{
    public class CameraMessageHandler : MonoBehaviour
    {
        #region ATTRIBUTES

        public StarFieldController StarField;
        public CameraFollow CamFollow;

        #endregion

        #region PUBLIC MESSAGES

        public void StartBackground()
        {
            CamFollow.JmpToObj();
            StarField.isRunning = true;
        }

        public void StopBackground()
        {
            StarField.isRunning = false;
        }

        #endregion
    }
}
