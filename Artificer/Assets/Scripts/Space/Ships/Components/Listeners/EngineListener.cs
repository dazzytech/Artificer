using UnityEngine;
using System.Collections;

using Space.Ship.Components.Attributes;

namespace Space.Ship.Components.Listener
{
    public class EngineListener : ComponentListener
    {
    	EngineAttributes _attr;

        #region MONOBEHAVIOUR

        #endregion

        #region PUBLIC INTERACTION

        public override void Activate()
        {
            if(!_attr.emitter.emit)
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

            ComponentType = "Engines";
            _attr = GetComponent<EngineAttributes>();

            if(hasAuthority)
                _attr.engineMotion = transform.up;
        }

        protected override void RunUpdate()
        {
            _attr.engineVelocity = transform.up * _attr.engineVelocity.magnitude;
            rb.AddForce
                (_attr.engineVelocity * Time.deltaTime, ForceMode2D.Force);

            // for now drag is hardcoded
            if (!_attr.active)
            {
                _attr.engineVelocity *= 0.98f;
                if (Mathf.Abs(_attr.engineVelocity.magnitude) < .01f)
                    _attr.engineVelocity = Vector3.zero;

                rb.drag = 0.9f;
            }
            else
            {
                _attr.engineMotion = transform.up;
                _attr.engineVelocity += _attr.engineMotion * _attr.acceleration;
                if (Mathf.Abs(_attr.engineVelocity.magnitude) > _attr.maxSpeed)
                    _attr.engineVelocity = (_attr.engineVelocity.normalized
                                            * _attr.maxSpeed) * _attr.engineMotion.magnitude;
            }
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

