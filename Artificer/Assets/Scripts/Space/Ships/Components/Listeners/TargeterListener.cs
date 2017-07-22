using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Data.Space;
using Space;
using Space.Ship.Components.Attributes;


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
                
                _att.ShipAtts = transform.parent.GetComponent<ShipAttributes>();

                _att.Behaviour = (TargeterBehaviour)_att.Data.behaviour;

                _att.EngageFire = _att.Data.AutoFire;

                _att.homeForward = transform.parent.transform.up;

                // find targets if auto lock
                if (_att.Behaviour == TargeterBehaviour.AUTOLOCKFIRE)
                    StartCoroutine("FindArcTargets");
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
            /*_attr.currentTarget = null;
            if (_attr.ShipAtts.Targets.Count < 1)
                return false;

            float ShortestDistance = float.MaxValue;

            foreach (Transform t in _attr.ShipAtts.Targets)
            {
                // Find angle between
                float tAngle = Angle(transform, t, false);
                // between min and max angle
                if(tAngle < _attr.MinAngle || tAngle > _attr.MaxAngle)
                    continue;

                // Find distance between this and target
                float tDistance = Vector3.Distance
                    (transform.position, t.position);

                // if distance less than shortest value
                if(tDistance < ShortestDistance && tDistance < _attr.AttackRange)
                {
                    ShortestDistance = tDistance;
                    _attr.currentTarget = t;
                }
            }

            if (_attr.currentTarget != null)
                return true;
            else*/
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
                tAngle = Math.Angle(_att.ShipAtts.Head.transform,
                    Camera.main.ScreenToWorldPoint(Input.mousePosition), FindHomeAngle());

                lookVector = (_att.ShipAtts.Head.transform.position
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

        /*float Angle(Transform trans, Vector2 point)
        {
            Vector2 pos = trans.position;
            float angle = Mathf.Atan2(point.y - pos.y, point.x - pos.x) * 180 / Mathf.PI - 90;
            return Mathf.DeltaAngle(FindHomeAngle(), angle);
        }*/

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

        #region COROUTINE

        /// <summary>
        /// Finds targets within the targeters firing arc.
        /// </summary>
        private IEnumerator FindArcTargets()
        {
            // temp
            yield break;

            /*while (true)
            {
                RaycastHit2D[] hits =
                Physics2D.CircleCastAll(transform.position, _attr.AttackRange, Vector2.zero, 0, 1);
            
                foreach (RaycastHit2D hit in hits)
                {
                    // Only auto target heads of ships
                    // while no target grouping
                    if (_attr.ShipAtts.Targets.Contains(hit.collider.transform)
                        || hit.collider.transform.tag != "Head")
                        continue;

                    // detect if enemy
                    if (_attr.ShipAtts.AlignmentLabel == "player")
                    {
                        // use player relations
                        if (hit.transform.name
                            != "Enemy")
                            continue;
                    } else
                    {
                        // ai relation
                        if (_attr.ShipAtts.AlignmentLabel == "Enemy" && 
                            hit.transform.name == "Enemy")
                            continue;

                        if (_attr.ShipAtts.AlignmentLabel == "Friendly" &&
                            hit.transform.name != "Enemy" || hit.transform.tag == "Station")
                            continue;
                    }

                    ComponentListener comp = hit.collider.
                    transform.GetComponent<ComponentListener>();

                    if (comp != null)
                    {
                        // check not self targetting
                        if (!_attr.ShipAtts.Components.Contains(comp))
                        {
                            // Find angle between
                            float tAngle = Angle(transform, hit.collider.transform, false);
                            if (tAngle > _attr.MinAngle || tAngle < _attr.MaxAngle)
                                _attr.ShipAtts.Targets.Add(hit.collider.transform);
                        }
                    }

                    yield return null;
                }
                yield return null;
            }*/
        }

        #endregion
    }
}
