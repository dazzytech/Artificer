using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

// Arificer
using Data.Shared;
using Space.Ship;

namespace Space.UI.Tracker
{

    #region MARKER CONTAINER

    /// <summary>
    /// Contaier class for tracking information 
    /// regarding transforms
    /// </summary>
    public class Marker
    {
        public Transform trackedObj;
        public GameObject arrow;
        public GameObject box;
        public GameObject text;
        public float trackDist;
    }

    #endregion

    /// <summary>
    /// Tracks nearby transforms using tags
    /// to identify and displays in circle
    /// </summary>
    public class TrackerHUD : MonoBehaviour
    {
        //private ContractData _contract;
        //private List<Transform> _targets;

        #region MARKER LISTS

        List<Marker> _markers;
        List<Marker> _pendingDelete;

        #endregion

        #region UI HUD PREFABS

        public GameObject undefinedArrow;
        public GameObject undefinedBox;
        public GameObject stationArrow;
        public GameObject stationBox;
        public GameObject enemyArrow;
        public GameObject enemyBox;
        public GameObject friendlyArrow;
        public GameObject friendlyBox;
        public GameObject missionArrow;
        public GameObject missionBox;

        #endregion

        #region MISC ATTRIBUTES

        // Font used to display distance text
        public Font font;

        // radius of the UI circle
        float _radius = 100f;

        #endregion

        #region MONO BEHAVIOUR

        void Awake()
        {
            _markers = new List<Marker>();
            _pendingDelete = new List<Marker>();
            //_targets = new List<Transform>();
        }

        /*public void SetContactData(ContractData contract)
        {
            //_contract = contract;
        }*/

        public void AddUIPiece(Transform piece)
        {
            if (GameObject.FindGameObjectWithTag
                ("MainCamera") == null)
                return;

            // stations aren't added this way so piece is a ship
            Marker m = new Marker();
            m.trackedObj = piece;

            _markers.Add(m);
        }


        void LateUpdate()
        {
            // retreive the position of the main camera in our scene
            Transform cameraTransform =
                GameObject.FindGameObjectWithTag
                    ("MainCamera").transform;

            // If we don't have a camera or anything to track
            // this stops here
            if (cameraTransform == null || _markers == null)
                return;

            int index = 0;

            foreach (Marker m in _markers)
            {
                if (m.trackedObj == null)
                {
                    _pendingDelete.Add(m);
                    if (m.arrow != null) m.arrow.SetActive(false);
                    if (m.box != null) m.box.SetActive(false);
                    if (m.text != null) m.text.SetActive(false);
                    continue;
                }

                if (m.arrow == null || m.box == null
                   || m.text == null)
                    BuildGUIPiece(m);

                float objDistance = SetPieceVisiblity(m, cameraTransform.position);

                Vector2 dir =
                    (m.trackedObj.position - cameraTransform.position)
                        .normalized * _radius;
                m.arrow.transform.up = dir.normalized;
                m.arrow.transform.localPosition = dir;


                //now you can set the position of the ui element
                m.box.GetComponent<RectTransform>().anchoredPosition =
                    UIConvert.WorldToCamera(m.trackedObj);
                m.text.transform.localPosition = dir + dir.normalized * Overlap(index, m, cameraTransform.position);
                m.text.GetComponent<Text>().text = ((int)objDistance / 10).ToString() + "km";

                index++;
            }

            foreach (Marker m in _pendingDelete)
            {
                _markers.Remove(m);
            }
        }


        #endregion

        private int Overlap(int index, Marker m, Vector3 cameraPosition)
        {
            int overlap = 30;

            Vector2 dirA =
                (m.trackedObj.position - cameraPosition);

            for (; index > 0;)
            {
                Marker testM = _markers[--index];
                if (testM.trackedObj == null)
                    continue;
                else if (!testM.arrow.activeSelf)
                    continue;

                Vector2 dirB =
                    (testM.trackedObj.position - cameraPosition);

                float angle = Vector2.Angle(dirA, dirB);
                if (angle < 10f)
                {
                    overlap += 30;
                }
            }

            return overlap;
        }

        public void RebuildGUI()
        {
            foreach (Marker m in _markers)
            {
                if (m.trackedObj.tag != "Station")
                {
                    Destroy(m.arrow);
                    m.arrow = null;
                    Destroy(m.box);
                    m.box = null;
                    Destroy(m.text);
                    m.text = null;
                }
            }
        }

        /// <summary>
        /// Sets the piece arrow and box visiblity.
        /// </summary>
        /// <returns>The piece distance to the object.</returns>
        /// <param name="m">M.</param>
        private float SetPieceVisiblity(Marker m, Vector3 camPos)
        {
            float objDistance = Vector3.Distance(m.trackedObj.position, camPos);
            Bounds camB = CameraExtensions.OrthographicBounds(Camera.main);
            if (camB.Contains(m.trackedObj.position))
            {
                m.arrow.SetActive(false);
                m.box.SetActive(true);
                m.text.SetActive(false);
            }
            else
            {
                if (m.trackDist == -1 || objDistance <= m.trackDist)
                {
                    m.arrow.SetActive(true);
                    m.text.SetActive(true);
                }
                else
                {
                    m.arrow.SetActive(false);
                    m.text.SetActive(false);
                }
                m.box.SetActive(false);
            }

            return objDistance;
        }

        private void BuildGUIPiece(Marker m)
        {
            if (m.trackedObj.tag == "Station")
            {
                Vector2 dir = (m.trackedObj.position
                    - playerTransform.position).normalized * _radius;

                m.text = new GameObject();
                Text mtext = m.text.AddComponent<Text>();
                m.text.transform.SetParent(this.transform, false);
                mtext.font = font;
                mtext.alignment = TextAnchor.MiddleCenter;
                mtext.fontSize = 8;

                m.arrow = (GameObject)Instantiate(stationArrow, dir, Quaternion.identity);
                m.arrow.transform.SetParent(this.transform, false);
                m.box = (GameObject)Instantiate(stationBox, dir, Quaternion.identity);
                m.box.transform.SetParent(this.transform, false);
                mtext.color = Color.yellow;
                m.trackDist = -1f;
            }
            else
            {
                m.trackDist = 500f;

                Vector2 dir = (m.trackedObj.position
                                   - playerTransform.position).normalized * _radius;
                m.text = new GameObject();
                Text text = m.text.AddComponent<Text>();
                m.text.transform.SetParent(this.transform, false);
                text.font = font;
                text.alignment = TextAnchor.MiddleCenter;
                text.fontSize = 8;

                // add different arrow depending
                string relation = m.trackedObj.name;

                switch (relation)
                {
                    case "Friendly":
                    case "Neutral":
                        /*foreach (MissionData md in _contract.PrimaryMissions)
                        {
                            if (md is EscortMission)
                            {
                                EscortMission t = md as EscortMission;
                                if(t.Targets.Contains(m.trackedObj))
                                {
                                    m.arrow = (GameObject)Instantiate(missionArrow, dir, Quaternion.identity);
                                    m.arrow.transform.SetParent(this.transform, false);
                                    m.box = (GameObject)Instantiate(missionBox, dir, Quaternion.identity);
                                    m.box.transform.SetParent(this.transform, false);
                                    m.box.SetActive(false);
                                    m.arrow.SetActive(false);
                                    text.color = Color.magenta;
                                    return;
                                }
                            }
                        }

                        m.arrow = (GameObject)Instantiate(friendlyArrow, dir, Quaternion.identity);
                        m.arrow.transform.SetParent(this.transform, false);
                        m.box = (GameObject)Instantiate(friendlyBox, dir, Quaternion.identity);
                        m.box.transform.SetParent(this.transform, false);
                        m.box.SetActive(false);
                        m.arrow.SetActive(false);
                        text.color = Color.green;*/
                        break;
                    case "Enemy":
                        /*foreach (MissionData md in _contract.PrimaryMissions)
                        {
                            if (md is TargetMission)
                            {
                                TargetMission t = md as TargetMission;
                                if(t.Targets.Contains(m.trackedObj))
                                {
                                    m.arrow = (GameObject)Instantiate(missionArrow, dir, Quaternion.identity);
                                    m.arrow.transform.SetParent(this.transform, false);
                                    m.box = (GameObject)Instantiate(missionBox, dir, Quaternion.identity);
                                    m.box.transform.SetParent(this.transform, false);
                                    m.box.SetActive(false);
                                    m.arrow.SetActive(false);
                                    text.color = Color.magenta;
                                    return;
                                }
                            }
                        }*/
                        m.arrow = (GameObject)Instantiate(enemyArrow, dir, Quaternion.identity);
                        m.arrow.transform.SetParent(this.transform, false);
                        m.box = (GameObject)Instantiate(enemyBox, dir, Quaternion.identity);
                        m.box.transform.SetParent(this.transform, false);
                        m.box.SetActive(false);
                        m.arrow.SetActive(false);
                        text.color = Color.red;
                        break;
                }
            }


        }

        #region MARKER UTILITIES

        /// <summary>
        /// Clears away all hud elements of the marker
        /// and adds it to the pending deleted
        /// </summary>
        /// <param name="m"></param>
        private void DeleteMarker(Marker m)
        {

        }

        #endregion
    }
}
