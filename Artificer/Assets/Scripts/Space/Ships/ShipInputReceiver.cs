using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
// Artificer Defined
using Space.Ship;
using Space.Ship.Components.Listener;
using Space.Ship.Components.Attributes;
using Space.UI.Ship;

namespace Space.Ship
{

    [RequireComponent(typeof(ShipAttributes))]
    public class ShipInputReceiver : NetworkBehaviour {

        #region ATTRIBUTES

        // ship references
        ShipAttributes _ship;
        ShipMessageController _msg;

        // delays key when switching combat states
        bool inputDelay;

        #endregion

        #region MONO BEHAVIOUR

        void Awake()
        {
            _ship = GetComponent<ShipAttributes>();
            _msg = GetComponent<ShipMessageController>();

            inputDelay = false;

            GetComponent<Rigidbody2D>().angularDrag = 0.9f;

            _ship.TargetedShips = new List<ShipSelect>();
            _ship.TargetDistance = 300f;
        }

        void Update()
        {
            if (_ship == null)
                return;

            List<Transform> remove = new List<Transform>();

            // Remove targets if they are null
            // Or are further away than our targeting distance
            for(int iBase = 0; iBase < _ship.TargetedShips.Count; iBase++)
            {
                ShipSelect selected = _ship.TargetedShips[iBase];

                if (selected.Ship == null)
                {
                    // ship has been destroyed
                    // remove this selection and revert index for
                    // component
                    _ship.TargetedShips.RemoveAt(iBase);
                    selected = null;
                    iBase--;
                    continue;
                }

                // Ship is still alive, however detect if
                // ship is still in tracking range
                if (Vector3.Distance(transform.position, 
                    selected.Ship.transform.position)
                   > _ship.TargetDistance)
                {
                    // We are out of tracking range
                    // remove this selection and revert index for
                    // component
                    _ship.TargetedShips.RemoveAt(iBase);
                    selected = null;
                    iBase--;
                    continue;
                }

                // If the ship is still being tracked then 
                // check if we are still tracking components
                for (int iComp = 0; iComp < selected.TargetedComponents.Count; iComp++)
                {
                    Transform component = selected.TargetedComponents[iComp];

                    // check the component is destroyed
                    if (component == null)
                    {
                        // if so then delete this item
                        // revert index
                        selected.TargetedComponents.RemoveAt(iComp);
                        component = null;
                        iComp--;
                        continue;
                    }
                }

                // Are there components left?
                if (selected.TargetedComponents.Count <= 0)
                {
                    // if not then we don't care about this ship
                    // remove this selection and revert index for
                    // component
                    _ship.TargetedShips.RemoveAt(iBase);
                    selected = null;
                    iBase--;
                    continue;
                }
            }
            

            // Remove self targeted components if null
            /*foreach (Transform t in _ship.SelfTargeted)
            {
                if (t == null)
                {
                    remove.Add(t);
                    continue;
                }
            }

            // Remove Targets
            foreach (Transform t in remove)
            {
                _ship.SelfTargeted.Remove(t);
            }*/
        }

        #endregion

        #region KEYBOARD INPUT

        /// <summary>
        /// Processes a single key.
        /// relays to main receiver
        /// </summary>
        /// <param name="key">Key.</param>
        public void ReceiveKey(KeyCode key)
        {
            List<KeyCode> keys = new List<KeyCode>();
            keys.Add(key);
            ReceiveKey(keys);
        }

        /// <summary>
        /// Receives a list of keys that the player pressed
        /// and compares them against all components' triggers
        /// Activates components player intends to activate
        /// </summary>
        /// <param name="keys">Pressed Keys</param>
        public void ReceiveKey(List<KeyCode> keys)
        {
            if (Time.timeScale == 0)
                return;

            foreach (KeyCode key in keys)
            {
                if (key == KeyCode.None)
                    return;

                if (key == Control_Config.GetKey("eject", "ship"))
                {
                    _msg.ShipDestroyed();
                }

                if (!inputDelay)
                {
                    if (key == Control_Config.GetKey("switchtocombat", "ship"))
                    {
                        _ship.Ship.CombatActive = !_ship.Ship.CombatActive;

                        // Switch off every component to stop sticking
                        foreach (ComponentListener listener in _ship.Components)
                            listener.Deactivate();

                        inputDelay = true;
                        StartCoroutine("EngageDelay");
                    }
                }

                // Components
                if (_ship.Ship.CombatActive)
                {
                    foreach (ComponentListener listener in _ship.Components)
                    {
                        ComponentAttributes att = listener.GetAttributes();
                        if (att != null && !att.active)
                        {
                            if (!(listener is WeaponListener) && !(listener is LauncherListener))
                            {
                                if (!_ship.Ship.CombatResponsive)
                                {
                                    if (att.TriggerKey == key)
                                    {
                                        listener.Activate();
                                    }

                                }
                                else
                                {
                                    if (att.CombatKey != KeyCode.None)
                                    {
                                        if (att.CombatKey == key)
                                        {
                                            listener.Activate();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // Not every component will have a combat key
                                if (att.CombatKey != KeyCode.None)
                                {
                                    if (att.CombatKey == key)
                                    {
                                        listener.Activate();
                                    }
                                }
                            }
                        }
                    }
                    return;
                }

                foreach (ComponentListener listener in _ship.Components)
                {
                    if (listener != null)
                    {
                        ComponentAttributes att = listener.GetAttributes();

                        if (att != null && !att.active)
                            if (att.TriggerKey == key)
                            {
                                listener.Activate();
                            }
                    }
                }

                
            }
        }

        /// <summary>
        /// Receives a list of keys that are released
        /// and deactivates any components that triggers
        /// correspond with the released keys
        /// </summary>
        /// <param name="key"></param>
        public void ReleaseKey(KeyCode key)
        {
            if (key != KeyCode.None)
            {
                // Components
                if (_ship.Ship.CombatActive)
                {
                    foreach (ComponentListener listener in _ship.Components)
                    {
                        ComponentAttributes att = listener.GetAttributes();

                        if (att != null && att.active)
                        {
                            if (!(listener is WeaponListener) && !(listener is LauncherListener))
                            {
                                if (!_ship.Ship.CombatResponsive)
                                {
                                    if (listener.GetAttributes().TriggerKey == key)
                                    {
                                        listener.Deactivate();
                                    }
                                }
                                else
                                {
                                    // Not every component will have a combat key
                                    if (att.CombatKey != KeyCode.None)
                                    {
                                        if (att.CombatKey == key)
                                        {
                                            listener.Deactivate();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // Not every component will have a combat key
                                if (att.CombatKey != KeyCode.None)
                                {
                                    if (att.CombatKey == key)
                                    {
                                        listener.Deactivate();
                                    }
                                }
                            }
                        }
                    }
                    return;
                }

                foreach (ComponentListener listener in _ship.Components)
                {
                    if (listener != null)
                    {
                        ComponentAttributes att = listener.GetAttributes();
                        if (att != null && att.active)
                            if (att.TriggerKey == key)
                            {
                                listener.Deactivate();
                            }
                    }
                }
            }
        }

        #endregion

        #region MOUSE INPUT

        /// <summary>
        /// Process a single click from the player input
        /// 
        /// </summary>
        /// <param name="position">Position.</param>
        public void SingleClick(Vector2 position)
        {
            if (!inputDelay)
            {
                if (_ship.Ship.CombatActive)
                    return;

                Collider2D hit =
                    Physics2D.OverlapPoint
                        (position);

                if (hit != null)
                {
                    _msg.AddTarget(hit.transform);
                }
                inputDelay = true;
                StartCoroutine("EngageDelay");
            }

        }

        /// <summary>
        /// Delete all the targets within the ships attributes
        /// </summary>
        public void ClearTargets()
        {
            if (!inputDelay)
            {
                _ship.TargetedShips.Clear();
                inputDelay = true;
                StartCoroutine("EngageDelay");
            }
        }

        #endregion

        #region COROUTRINES

        private IEnumerator EngageDelay()
        {
            yield return new WaitForSeconds(1f);
            inputDelay = false;
            yield return null;
        }

        #endregion
    }
}
