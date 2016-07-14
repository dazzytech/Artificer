using UnityEngine;
using System.Collections;

using Space.Ship.Components.Attributes;

namespace Space.Ship.Components.Listener
{
    public class WarpListener : ComponentListener 
    {
        WarpAttributes _attr;
        bool warping;
        
        void Awake()
        {
            ComponentType = "Warps";
            _attr = GetComponent<WarpAttributes>();
        }
        
        void Start ()
        {
            base. SetRB();
            _attr.readyToFire = true;
            warping = false;
        }
    	
    	// Update is called once per frame
    	void Update () {
    	    if (_attr.readyToFire)
                _attr.TimeCount = 0;
            else
                _attr.TimeCount += Time.deltaTime;
    	}

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

        public override void Deactivate()
        {
            
        }

        private IEnumerator EngageDelay()
        {
            yield return new WaitForSeconds (_attr.WarpDelay);
            _attr.readyToFire = true;
            yield return null;
        }
        
        public void SetTriggerKey(string key)
        {
            _attr.TriggerKey = Control_Config
                .GetKey(key, "ship");
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
    }
}
