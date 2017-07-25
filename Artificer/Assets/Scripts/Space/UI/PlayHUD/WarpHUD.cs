using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Artificer Defined
using Space.Ship;
using Space.Ship.Components.Listener;
using Space.Ship.Components.Attributes;

namespace Space.UI.Ship
{
    public class WarpHUD : MonoBehaviour
    {
        // Ship attributes for player ship
        private ShipAccessor _shipRef;

        // HUD elements
        Slider WarpBooster;

        // HUD Panels
        private Transform _WarpHUD;

        void Awake()
        {
            _WarpHUD = transform.Find("WarpHUD");
            WarpBooster = _WarpHUD.Find("WarpDistanceSlider").GetComponent<Slider>();
            WarpBooster.gameObject.SetActive(false);
        }

        public void SetShipData(ShipAccessor data)
        {
            _shipRef = data;
        }
    	
        // Update is called once per frame
        void Update ()
        {
            // Do not update if ship data is null
            if (_shipRef == null)
            {
                // hide HUD
                if(_WarpHUD.gameObject.activeSelf)
                    _WarpHUD.gameObject.SetActive(false);
                return; 
            }
            else
                // show HUD if hidden
                if(!_WarpHUD.gameObject.activeSelf)
                    _WarpHUD.gameObject.SetActive(true);

            if (_shipRef.Warp == null)
            {
                if (WarpBooster.gameObject.activeSelf)
                    WarpBooster.gameObject.SetActive(false);
            }
            else
            {
                if (!WarpBooster.gameObject.activeSelf)
                    WarpBooster.gameObject.SetActive(true);
                UpdateWarpState();
            }
        }

        /// <summary>
        /// Builds the warp HUD.
        /// show jump distance and jump delay
        /// </summary>
        private void UpdateWarpState()
        {
            // Get warp text
            Text statusText =  _WarpHUD.Find("statusText")
                .GetComponent<Text>();

            // Detect ready to fire 
            WarpAttributes att = _shipRef.Warp.GetAttributes()
                as WarpAttributes;

            if (att.readyToFire)
                statusText.text = "Warp Ready To Fire";
            else
                statusText.text = "Next Warp Jump In: " + 
                    (att.WarpDelay - att.TimeCount).ToString("F1") + "s";

            // Determine values
            Text minText = WarpBooster.transform.Find
                ("minVal").GetComponent<Text>();
            minText.text = ((int)att.MinDistance/10).ToString()+"km";
            Text maxText = WarpBooster.transform.Find
                ("maxVal").GetComponent<Text>();
            maxText.text = ((int)att.MaxDistance/10).ToString()+"km";
            Text dist = WarpBooster.transform.Find
                ("Distance").GetComponent<Text>();
            dist.text = ((int)att.WarpDistance/10).ToString()+"km";
        }

        /// <summary>
        /// Slider trigger to set 
        /// the warp distance
        /// Shouldn't be possible to be called
        /// if no warp
        /// </summary>
        public void BoosterChange()
        {
            // Saftey check
            if (_shipRef.Warp == null)
                return;

            // Calc value
            WarpAttributes att = _shipRef.Warp.GetAttributes() as WarpAttributes;
            float step = (att.MaxDistance - att.MinDistance) *.1f;
            float finalValue = step * WarpBooster.value;
            // Set value
            att.WarpDistance = att.MinDistance + finalValue;
        }
    }
}
