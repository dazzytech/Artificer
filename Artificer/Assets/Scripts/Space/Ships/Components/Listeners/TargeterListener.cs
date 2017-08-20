using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Data.Space;
using Space;
using Space.Ship.Components.Attributes;
using Space.UI.Ship;
using Space.UI.Ship.Target;

namespace Space.Ship.Components.Listener
{
    public class TargeterListener : ComponentListener
    {
        TargeterAttributes _att;

        #region PUBLIC INTERACTION

        public override void Activate()
        {
            // go through each connected piece and activate them
            if (_att.ComponentList.Count == 0)
                PopulateComponentList();
            foreach (ComponentListener comp in _att.ComponentList)
            {
                if(comp != null)
                    comp.Activate();
            }
        }

        public void SetTargeting(bool fire, int behaviour)
        {
            _att.EngageFire = fire;
            if (behaviour != -1)
                _att.Behaviour = (TargeterBehaviour)behaviour;

            // find targets if auto lock
            if (_att.Behaviour == TargeterBehaviour.AUTOLOCKFIRE)
                StartCoroutine(FindArcTargets
                    (_att.AttackRange, _att.MinAngle, _att.MaxAngle, _att.Ship));
            else
                StopAllCoroutines();
        }

        #endregion

        #region PRIVATE UTILITIES

        protected override void InitializeComponent()
        {
            base.InitializeComponent();

            ComponentType = "Targeter";
            _att = GetComponent<TargeterAttributes>();

            if (hasAuthority)
            {
                _att.ComponentList = new List<ComponentListener>();
              
                _att.Behaviour = (TargeterBehaviour)_att.Data.Behaviour;

                _att.EngageFire = _att.Data.AutoFire;

                _att.homeForward = transform.parent.transform.up;

                // find targets if auto lock
                if (_att.Behaviour == TargeterBehaviour.AUTOLOCKFIRE)
                    StartCoroutine(FindArcTargets
                        (_att.AttackRange, _att.MinAngle, _att.MaxAngle, _att.Ship));
            }
        }

        // Update is called once per frame
        protected override void RunUpdate()
        {
            // Auto engage targets if not a mouse follow behaviour
            if (_att.Behaviour != TargeterBehaviour.MOUSECONTROL)
            {
                // Search for target to shoot
                if (FindClosestTarget())
                {
                    LerpTowardsTarget();
                    if (_att.EngageFire)
                    {
                        // if angles are between a certain arc then 
                        // activate connected objects
                        // Find angle between this and target
                        float tAngle = Math.Angle(transform, _att.currentTarget.position);
                        // between min and max angle of 15 then we fire
                        if (tAngle < 5f && tAngle > -5f)
                            if (Vector3.Distance(transform.position,
                            _att.currentTarget.position) <= _att.AttackRange)
                                Activate();
                    }
                }
                else
                {
                    LerpTowardsFace();
                }
            }
            else
            {
                // Control the mousefollow behaviour here
                if (_att.ShipData.CombatActive)
                {
                    LerpTowardsMouse();
                }
                else
                {
                    LerpTowardsFace();
                }
            }

            // Keep all components fixed to targeter
            if (_att.ComponentList.Count == 0)
                PopulateComponentList();
            foreach (ComponentListener comp in _att.ComponentList)
            {
                if (comp != null)
                    comp.FixToConnected();
            }
        }

        #region TARGETTING

        /// <summary>
        /// Finds the closest target to the ship.
        /// </summary>
        /// <returns><c>true</c>,
        /// if closest target was found, 
        /// <c>false</c> otherwise.</returns>
        private bool FindClosestTarget()
        {
            _att.currentTarget = null;
            if (_att.Ship.TargetedShips.Count < 1)
                return false;

            float ShortestDistance = float.MaxValue;

            foreach (ShipSelect ship in _att.Ship.TargetedShips)
            {
                foreach (Transform t in ship.TargetedComponents)
                {
                    // Find angle between
                    float tAngle = Math.Angle(transform, t.position, FindHomeAngle());
                    // between min and max angle
                    if (tAngle < _att.MinAngle || tAngle > _att.MaxAngle)
                        continue;

                    // Find distance between this and target
                    float tDistance = Vector3.Distance
                        (transform.position, t.position);

                    // if distance less than shortest value
                    if (tDistance < ShortestDistance && tDistance < _att.AttackRange)
                    {
                        ShortestDistance = tDistance;
                        _att.currentTarget = t;
                    }
                }
            }

            if (_att.currentTarget != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Lerps the targeter angle towards the target.
        /// </summary>
        private void LerpTowardsTarget()
        {
            Quaternion turretRotation =
                Quaternion.LookRotation((transform.position
                                        - _att.currentTarget.position), transform.TransformDirection
                                        (Vector3.forward));

            transform.rotation =
                Quaternion.Slerp(transform.rotation,
                                  new Quaternion(0, 0, turretRotation.z, turretRotation.w),
                                  Time.deltaTime * _att.turnSpeed);
        }

        /// <summary>
        /// NOT-TARGETING
        /// </summary>
        private void LerpTowardsFace()
        {
            Quaternion turretRotation =
                Quaternion.LookRotation(-FindHomeUp(),
                transform.TransformDirection
                (Vector3.forward));

            transform.rotation =
                Quaternion.Slerp(transform.rotation,
                new Quaternion(0, 0, turretRotation.z, turretRotation.w),
                Time.deltaTime * _att.turnSpeed);
        }

        /// <summary>
        /// find the angle difference between the mouse and 
        /// the component
        /// </summary>
        private void LerpTowardsMouse()
        {
            // get the distance between self and mouse
            float distance = Vector3.Distance(transform.position,
                Camera.main.ScreenToWorldPoint(Input.mousePosition));

            // Init our angle
            float tAngle = 0;

            Vector2 lookVector = Vector2.zero;

            if (distance < _att.MinFollow)
            {
                // base our angle on the ship head
                tAngle = Math.Angle(_att.Ship.Head.transform,
                    Camera.main.ScreenToWorldPoint(Input.mousePosition), FindHomeAngle());

                lookVector = (_att.Ship.Head.transform.position
                - Camera.main.ScreenToWorldPoint(Input.mousePosition));

            }
            else
            {
                // follow the mouse directly
                tAngle = Math.Angle(transform, Camera.main.ScreenToWorldPoint
                    (Input.mousePosition), FindHomeAngle());
                lookVector = (transform.position
                - Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }

            // between min and max angle
            if (tAngle < _att.MinAngle || tAngle > _att.MaxAngle)
            {
                _att.Aligned = false;
                return;
            }

            Quaternion turretRotation =
                Quaternion.LookRotation(lookVector, transform.TransformDirection
                (Vector3.forward));

            transform.rotation =
                Quaternion.Slerp(transform.rotation,
                new Quaternion(0, 0, turretRotation.z, turretRotation.w),
                Time.deltaTime * _att.turnSpeed);

            float diff = Mathf.Abs(_att.PrevAngle - transform.rotation.z);
            if (diff < 0.005f)
                _att.Aligned = true;
            else
                _att.Aligned = false;

            _att.PrevAngle = transform.rotation.z;
        }

        #endregion

        #region UTILITIES 

        /// <summary>
        /// Finds the origin vector of the component
        /// </summary>
        private Vector3 FindHomeUp()
        {
            Vector3 homeUp = transform.parent.up;

            if (_att.homeForward == Vector3.down)
                homeUp = -transform.parent.up;
            if (_att.homeForward == Vector3.left)
                homeUp = -transform.parent.right;
            if (_att.homeForward == Vector3.right)
                homeUp = transform.parent.right;

            return homeUp;
        }

        private float FindHomeAngle()
        {
            float euler = transform.parent.eulerAngles.z;

            if (_att.homeForward == Vector3.down)
                euler += 180;
            if (_att.homeForward == Vector3.left)
                euler += 90;
            if (_att.homeForward == Vector3.right)
                euler -= 90;

            return euler;
        }

        /// <summary>
        /// Adds any listeners of a certain type
        /// to the component list
        /// </summary>
        private void PopulateComponentList()
        {
            foreach (ComponentListener comp in _att.connectedComponents)
            {
                // tracker type - weapon
                if (comp is WeaponListener)
                {
                    _att.ComponentList.Add(comp);
                    // Clear triggers if weapon is not mouse controlled
                    if (_att.Behaviour != TargeterBehaviour.MOUSECONTROL)
                    {
                        comp.GetAttributes().TriggerKey = KeyCode.None;
                        comp.GetAttributes().CombatKey = KeyCode.None;
                    }
                }
            }
        }

        #endregion

        #endregion
    }
}
