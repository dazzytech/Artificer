using UnityEngine;
using System.Collections;

using Space.Ship.Components.Attributes;

namespace Space.Ship.Components.Listener
{
    public class WarpListener : ComponentListener 
    {
        #region ATTRIBUTES

        WarpAttributes _attr;
        bool warping;

        #endregion

        #region PUBLIC INTERACTION

        public override void Activate()
        {
            if (_attr.readyToFire && !warping)
            {
                // Create warp affects and start the warp warm up proccess
                StartCoroutine("FireWarp");
                warping = true;
                GameObject warpFX = (GameObject)Instantiate(_attr.WarpFXPrefab, 
                                      transform.position, Quaternion.identity);
                warpFX.transform.parent = transform;
            }
        }

        #endregion

        #region PRIVATE UTILITIES

        protected override void InitializeComponent()
        {
            base.InitializeComponent();

            ComponentType = "Warps";
            _attr = GetComponent<WarpAttributes>();

            if (hasAuthority)
            {
                _attr.readyToFire = true;
                warping = false;
            }
        }
        
        // Update is called once per frame
        protected override void RunUpdate()
        {
            if (_attr.readyToFire)
                _attr.TimeCount = 0;
            else
                _attr.TimeCount += Time.deltaTime;
        }

        #endregion

        #region COROUTINES

        private IEnumerator EngageDelay()
        {
            yield return new WaitForSeconds (_attr.WarpDelay);
            _attr.readyToFire = true;
            yield return null;
        }

        IEnumerator FireWarp()
        {
            yield return new WaitForSeconds(_attr.WarpWarmUp);

            Vector3 forward = transform.TransformDirection(Vector3.up);
            _attr.readyToFire = false;
            warping = false;
            Vector3 newPos = transform.parent.transform.position +
                (transform.parent.up*_attr.WarpDistance);
            transform.parent.transform.position = newPos;
            
            StartCoroutine("EngageDelay");
        }

        #endregion
    }
}
