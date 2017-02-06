﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

// Arificer
using Data.Shared;
using Space.Ship;
using UI.Effects;

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

        private List<Marker> _markers;
        private List<Marker> _pendingDelete;

        #endregion

        #region UI HUD PREFABS

        [Header("Arrow")]
        [SerializeField]
        private GameObject m_arrowPrefab;

        [Header("Box")]
        [SerializeField]
        private GameObject m_boxPrefab;

        /*public GameObject stationArrow;
        public GameObject stationBox;
        public GameObject enemyArrow;
        public GameObject enemyBox;
        public GameObject friendlyArrow;
        public GameObject friendlyBox;
        public GameObject missionArrow;
        public GameObject missionBox;*/

        #endregion

        #region COLOUR

        [Header("Friendly")]
        [SerializeField]
        private Color m_friendlyColor;

        [Header("Enemy")]
        [SerializeField]
        private Color m_enemyColor;

        #endregion

        #region MISC ATTRIBUTES

        // Font used to display distance text
        public Font font;

        // radius of the UI circle
        float _radius = 100f;

        #endregion

        #region MONO BEHAVIOUR

        void LateUpdate()
        {
            GameObject cameraObject = GameObject.FindGameObjectWithTag
                    ("MainCamera");

            // If we don't have a camera or anything to track
            // this stops here
            if (cameraObject == null || _markers == null)
                return;
            
            // retreive the position of the main camera in our scene
            Transform cameraTransform = cameraObject.transform;

            int i = 0;

            foreach (Marker m in _markers)
            {
                if (m.trackedObj == null)
                {
                    DeleteMarker(m);
                    continue;
                }

                if (m.arrow == null || m.box == null
                   || m.text == null)
                    BuildMarker(m, cameraTransform.position);

                RepositionMarker(m, cameraTransform.position, i);

                i++;
            }

            if (_pendingDelete == null)
                return;

            foreach (Marker m in _pendingDelete)
            {
                _markers.Remove(m);
            }
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Called externally when theres an object we want to track
        /// typically by the spawner
        /// </summary>
        /// <param name="piece"></param>
        public void AddUIPiece(Transform piece)
        {
            // stations aren't added this way so piece is a ship
            Marker m = new Marker();
            m.trackedObj = piece;

            if (_markers == null)
                _markers = new List<Marker>();

            _markers.Add(m);
        }

        #endregion

        #region INTERNAL FUNCTIONS

        /// <summary>
        /// If the angles of the two
        /// objects are about to cause the
        /// text to overlap: push one forward
        /// </summary>
        /// <param name="index"></param>
        /// <param name="m"></param>
        /// <param name="cameraPosition"></param>
        /// <returns></returns>
        private int Overlap(int index, Marker m, Vector3 cameraPosition)
        {
            int overlap = 40;

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
                    overlap += 40;
                }
            }

            return overlap;
        }

        #endregion

        #region MARKER UTILITIES

        /// <summary>
        /// Clears away all hud elements of the marker
        /// and adds it to the pending deleted
        /// </summary>
        /// <param name="m"></param>
        private void DeleteMarker(Marker m)
        {
            if (_pendingDelete == null)
                _pendingDelete = new List<Marker>();

            // pending markers will be deleted at the end of 
            // the update
            _pendingDelete.Add(m);

            // if any UI elements exist, disable them until deletion
            if (m.arrow != null) m.arrow.SetActive(false);
            if (m.box != null) m.box.SetActive(false);
            if (m.text != null) m.text.SetActive(false);
        }

        /// <summary>
        /// Constructs the marker based on the objects tag
        /// and places in the correct position around the ship
        /// </summary>
        /// <param name="m"></param>
        /// <param name="camPos"></param>
        private void BuildMarker(Marker m, Vector3 camPos)
        {
            // face the marker pointing towards the object
            Vector2 dir = (m.trackedObj.position
                    - camPos).normalized * _radius;

            // Generic text object within marker to display
            // the distance to the object 
            m.text = new GameObject();

            // Add text component
            Text mtext = m.text.AddComponent<Text>();
            
            // Place the text as a child of this HUD
            m.text.transform.SetParent(this.transform, false);

            // Initialize text format 
            mtext.font = font;
            mtext.alignment = TextAnchor.MiddleCenter;
            mtext.fontSize = 8;

            // Initialize the HUD items
            m.arrow = (GameObject)Instantiate(m_arrowPrefab, dir, Quaternion.identity);
            m.arrow.transform.SetParent(this.transform, false);
            m.box = (GameObject)Instantiate(m_boxPrefab, dir, Quaternion.identity);
            m.box.transform.SetParent(this.transform, false);
            m.box.SetActive(false);
            m.arrow.SetActive(false);

            // assign UI elements based on tag (for now)
            switch (m.trackedObj.tag)
            {
                case "Station":
                    // NOT USED ATM
                    // for now assign yellow objects to all stations
                    //mtext.color = Color.yellow;
                    //m.trackDist = -1f;
                    break;
                case "Friendly":
                    // Colourize HUD elements
                    mtext.color = m_friendlyColor;
                    m.arrow.GetComponent<Image>().color = m_friendlyColor;
                    m.box.GetComponent<Image>().color = m_friendlyColor;
                    m.trackDist = 500f;
                    break;
                case "Enemy":
                    // Colourize HUD elements
                    mtext.color = m_enemyColor;
                    m.arrow.GetComponent<Image>().color = m_enemyColor;
                    m.box.GetComponent<Image>().color = m_enemyColor;
                    m.trackDist = 500f;
                    break;
                default:
                    // Grey if a different tag              
                    mtext.color = Color.grey;
                    m.trackDist = 500f;
                    break;
            }
        }

        /// <summary>
        /// Sets the piece arrow and box visiblity.
        /// depending on position and distance of tracked
        /// object.
        /// </summary>
        /// <returns>The piece distance to the object.</returns>
        /// <param name="m">M.</param>
        private float SetMarkerVisiblity(Marker m, Vector3 camPos)
        {
            // distance between our ship and tracked object
            float objDistance = Vector3.Distance(m.trackedObj.position, camPos);
            // Retreive borders of the screen game is being played on
            Bounds camB = CameraExtensions.OrthographicBounds(Camera.main);

            if (camB.Contains(m.trackedObj.position))
            {
                if (!m.box.activeSelf)
                {
                    // object is visible to use
                    // only show box directly over obj
                    m.arrow.SetActive(false);
                    m.box.SetActive(true);
                    m.text.SetActive(false);

                    //PanelFadeEffects.FlashInItem(m.box.GetComponent<Image>());
                }
            }
            else
            {
                // Object is offscreen
                // Display arrow if within tracking range
                if (m.trackDist == -1 || objDistance <= m.trackDist)
                {
                    if (!m.arrow.activeSelf)
                    {
                        m.arrow.SetActive(true);
                        m.text.SetActive(true);

                        //PanelFadeEffects.FlashInItem(m.arrow.GetComponent<Image>());
                    }
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

        /// <summary>
        /// respostion GUI Elements based on tracked object position
        /// </summary>
        /// <param name="m"></param>
        /// <param name="camPos"></param>
        /// <param name="i"></param>
        private void RepositionMarker(Marker m, Vector3 camPos, int i)
        {
            float objDistance = SetMarkerVisiblity(m, camPos);

            Vector2 dir =
                (m.trackedObj.position - camPos)
                    .normalized * _radius;
            m.arrow.transform.up = dir.normalized;
            m.arrow.transform.localPosition = dir;


            //now you can set the position of the ui element
            m.box.GetComponent<RectTransform>().anchoredPosition =
                UIConvert.WorldToCamera(m.trackedObj);
            m.text.transform.localPosition = dir + dir.normalized * Overlap(i, m, camPos);
            m.text.GetComponent<Text>().text = ((int)objDistance * 0.01).ToString("F2") + "km";
        }

        #endregion
    }
}
