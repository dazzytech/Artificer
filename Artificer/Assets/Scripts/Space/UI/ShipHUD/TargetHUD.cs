using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using ShipComponents;

namespace Space.UI.Target
{
    public class Marker
    {
        public Transform trackedObj;
        public GameObject Icon;
    }

    public class TargetHUD : MonoBehaviour
    {
        private List<Marker> _markers;
        private List<Transform> _pending;
        private ShipAttributes _data;
        public GameObject _prefabOther;
        public GameObject _prefabSelf;
        public GameObject _prefabHighlight;
        public GameObject _selectionRect;

        bool targeting; 

        // TargeterHUD
        public Transform _targeterHUD;
        public Toggle _AutoFire;

        // Quick utlity to clear target list upon death
        void OnEnable()
        {
            SpaceManager.PlayerExitScene += PlayerDeath;
        }

        void OnDisable()
        {
            SpaceManager.PlayerExitScene += PlayerDeath;
        }

        // Use this for initialization
        public void SetShipData(ShipAttributes data)
        {
            _data = data;

            _markers = new List<Marker>();
            _pending = new List<Transform>();

            if (_data.Targeter.Count > 0)
            {
                if (_targeterHUD.gameObject.activeSelf == false)
                    _targeterHUD.gameObject.SetActive(true);
                targeting = true;
            } else if (_data.Launchers.Count > 0)
            {
                    if (_targeterHUD.gameObject.activeSelf == true)
                        _targeterHUD.gameObject.SetActive(false);
                    targeting = true;
            }else
                targeting = false;
        }

        void Update()
        {
            if (targeting)
            {
                UpdateTargeter();
            }
        }

        // Update is called once per frame
        void UpdateTargeter()
        {
            List<Marker> removeList = new List<Marker>();
            List<Transform> current = new List<Transform>();
            // find if currently tracked
            foreach(Marker m in _markers)
            {
                if(m.trackedObj != null)
                {
                    if(!_data.Targets.Contains(m.trackedObj)
                       && !_data.SelfTargeted.Contains(m.trackedObj)
                       && !_data.HighlightedTargets.Contains(m.trackedObj))
                        removeList.Add(m);

                    if(_pending.Contains(m.trackedObj))
                    {
                        if(!_data.HighlightedTargets.Contains(m.trackedObj))
                        {
                            removeList.Add(m);
                            _pending.Remove(m.trackedObj);
                        }
                    }

                    // blend markers that are together?
                    current.Add(m.trackedObj);

                }
                else
                {
                    removeList.Add(m);
                }
            }

            foreach (Marker m in removeList)
            {
                Destroy(m.Icon);
                _markers.Remove(m);
            }

            foreach (Transform t in _data.Targets)
            {
                if(!current.Contains(t))
                {
                    BuildPiece(t, _prefabOther);
                }
            }

            foreach (Transform t in _data.SelfTargeted)
            {
                if(!current.Contains(t))
                {
                    BuildPiece(t, _prefabSelf);
                }
            }

            foreach (Transform t in _data.HighlightedTargets)
            {
                if(!current.Contains(t))
                {
                    BuildPiece(t, _prefabHighlight);
                    _pending.Add(t);
                }
            }
        }

        private void BuildPiece(Transform t, GameObject prefab)
        {
            Marker m = new Marker();
            m.Icon = Instantiate(prefab);
            m.Icon .transform.SetParent(this.transform);
            m.Icon.GetComponent<RectTransform>().localPosition = Vector3.zero;
            m.Icon.GetComponent<RectTransform>().localScale = new Vector3(20f, 20f, 1f);
            m.trackedObj = t;
            _markers.Add(m);
        }

        void LateUpdate()
        {
            if (_data != null)
            {
                Vector2 startpoint = UIConvert.WorldToCameraRect(_data.HighlightRect);
                Vector2 endpoint = UIConvert.WorldToCameraRectEnd(_data.HighlightRect);

                if (endpoint.y > 0)
                {
                    _selectionRect.GetComponent<RectTransform>().offsetMin = startpoint;
                    _selectionRect.GetComponent<RectTransform>().offsetMax = endpoint;
                } else
                {
                    _selectionRect.GetComponent<RectTransform>().offsetMin = new Vector2(startpoint.x, endpoint.y);
                    _selectionRect.GetComponent<RectTransform>().offsetMax = new Vector2(endpoint.x, startpoint.y);
                }

                List<Marker> removeList = new List<Marker>();
                foreach (Marker m in _markers)
                {      
                    if (m.trackedObj != null)
                    {
                        //now you can set the position of the ui element
                        m.Icon.GetComponent<RectTransform>().anchoredPosition =
                        UIConvert.WorldToCamera(m.trackedObj);
                    } else
                        removeList.Add(m);
                }
                foreach (Marker m in removeList)
                {
                    Destroy(m.Icon);
                    _markers.Remove(m);
                }
            }
        }

        // Targeter HUD buttons value
        public void ChangeValue()
        {
            foreach (TargeterListener targ in _data.Targeter)
            {
                TargeterAttributes att = (TargeterAttributes)targ.GetAttributes();
                att.EngageFire = _AutoFire.isOn;
            }
        }


        private void PlayerDeath()
        {
            if (_markers != null)
            {
                // clear all targets
                foreach (Marker m in _markers)
                {
                    Destroy(m.Icon);
                }
                _markers.Clear();
            }
        }
    }
}
