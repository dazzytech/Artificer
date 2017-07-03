using UnityEngine;
using System.Collections;

using Space;
using Data.Shared;

using Space.Ship.Components.Attributes;

namespace Space.Ship.Components.Listener
{
    public class RotorListener : ComponentListener{

    	RotorAttributes _attr;

        #region PUBLIC FUNCTIONALITY

    	public void SetTriggerKey(string key)
    	{
    		_attr.TriggerKey =
    			Control_Config.GetKey(key, "ship");

    		_attr.turnVector = new Vector3();
    		_attr.turnVector.z = -transform.right.y;
    	}

        public override void Activate()
        {
            if (!_attr.emitter.emit)
                base.Activate();
        }

        public override void Deactivate()
        {
            if (_attr.emitter.emit)
                base.Deactivate();
        }

        public override void Destroy()
        {
            base.Destroy();
            
            Deactivate();
        }

        #endregion

        #region PRIVATE UTILITIES

        protected override void InitializeComponent()
        {
            base.InitializeComponent();

            ComponentType = "Rotors";
            _attr = GetComponent<RotorAttributes>();
        }

        protected override void RunUpdate()
        {
            // make sure we have assigned ship yet
            if (_attr.Ship == null)
                return;

            // Perform automated turning if in combat mode
            if (_attr.ShipData.CombatActive && _attr.ShipData.CombatResponsive)
            {
                float difference = CalcAngle();
                if (Mathf.Sign(transform.localEulerAngles.z - 180) < 0)
                {
                    if (difference < -5f)
                    {
                        Activate();
                    }
                    else
                        Deactivate();
                }
                else
                {
                    if (difference > 5f)
                    {
                        Activate();
                    }
                    else
                        Deactivate();
                }

                if (difference < 1f && difference > -1f)
                    rb.angularVelocity = 0f;
            }

            // Apply rotation force
            float turnAmount = _attr.turnSpeed * Mathf.Sign(transform.localEulerAngles.z - 180);

            rb.AddTorque(turnAmount * Time.deltaTime);


            // add friction
            if (!_attr.active)
            {
                _attr.turnSpeed = 0f;
                rb.angularDrag = 1f;
            }
            else
            {
                _attr.turnSpeed += _attr.turnAcceleration;

                if (Mathf.Abs(_attr.turnSpeed) > _attr.maxTurnSpeed)
                    _attr.turnSpeed = _attr.maxTurnSpeed;
            }
        }

        private float CalcAngle()
        {
            Vector2 dest = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 pos = transform.parent.position;
            float angle = Mathf.Atan2(dest.y-pos.y, dest.x-pos.x)*180 / Mathf.PI -90;
            return Mathf.DeltaAngle(transform.parent.eulerAngles.z, angle);
        }

        protected override void ActivateFx()
        {
            base.ActivateFx();

            _attr.emitter.emit = true;
            _attr.active = true;
        }

        protected override void DeactivateFx()
        {
            base.DeactivateFx();

            if (_attr.emitter != null)
            {
                _attr.emitter.emit = false;
                _attr.active = false;
            }
        }

        #endregion
    }
}