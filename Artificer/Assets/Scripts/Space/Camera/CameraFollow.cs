using UnityEngine;
using System.Collections;

namespace Space.CameraUtils
{
    /// <summary>
    /// Attached to a camera to follow an objectv smoothly
    /// </summary>
    public class CameraFollow : MonoBehaviour
    {
        #region ATTRIBUTES

        public Transform objToFollow;
        public Transform thisTransform;
        public float PosDamp = 4f;

        #endregion

        #region MONO BEHAVIOUR

        // Use this for initialization
        void Awake()
        {
            objToFollow = null;
            thisTransform = GetComponent<Transform>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (objToFollow != null)
            {
                //Get output velocity
                Vector3 Velocity = Vector3.zero;

                thisTransform.position =
                    Vector3.SmoothDamp(thisTransform.position,
                        objToFollow.position, ref Velocity,
                        PosDamp * Time.fixedDeltaTime);

                thisTransform.position =
                    new Vector3(thisTransform.position.x,
                        thisTransform.position.y, -10f);

            }
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Sets this camera to keep following passed object
        /// </summary>
        /// <param name="newObj"></param>
        public void SetFollowObj(Transform newObj)
        {
            if (newObj == null)
            {
                Debug.Log
                    ("CameraFollow - SetFollowObj: Object doesnt exist!");
                return;
            }

            objToFollow = newObj;
        }

        /// <summary>
        /// Move immediately to object camera is following
        /// </summary>
        public void JmpToObj()
        {
            thisTransform.position = objToFollow.position;
        }

        #endregion
    }
}

