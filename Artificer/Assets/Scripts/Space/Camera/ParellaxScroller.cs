using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Space.CameraUtils
{
    /// <summary>
    /// Item that will be scrolled in the background
    /// </summary>
    [Serializable]
    public struct ParellaxItem
    {
        public float X;
        public float Y;
        public float Distance;
        public string Texture;
        public string Type;
        [NonSerialized]
        public GameObject GO;
        [NonSerialized]
        public Bounds Bound;
    }

    public class ParellaxScroller : NetworkBehaviour
    {
        #region ATTRIBUTES

        // store all the items we will be scrolling
        private List<ParellaxItem> _scrollItems;

        #endregion

        #region MONO BEHAVIOUR

        void OnDestroy()
        {
            StopCoroutine("CycleScroller");
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Creates a scroller object
        /// and stores it in synced list across playercameras
        /// </summary>
        public void AddScrollObject(ParellaxItem newItem)
        {
            if (_scrollItems == null)
            {
                _scrollItems = new List<ParellaxItem>();
                StartCoroutine("CycleScroller");
            }

            // Create base game object for either type of object
            GameObject newObject = new GameObject();
            newObject.name = newItem.Type;
            // position object
            newObject.transform.parent = this.transform;
            newObject.transform.position = Vector2.zero;
            newObject.gameObject.layer = 9;

            // If object is a planet then change scale based on distance
            if (newItem.Type == "Planet")
            {
                // Build game object 
                Vector3 scale = new Vector3();
                float newScale = UnityEngine.Random.Range(.1f, .5f);
                scale = Vector3.one * newScale;
                Debug.Log(scale);
                newObject.transform.localScale = scale;
            }

            // Create visual element
            SpriteRenderer render = newObject.AddComponent<SpriteRenderer>();
            Sprite planetimg = Resources.Load(newItem.Texture, typeof(Sprite)) as Sprite;
            render.sprite = planetimg;
            render.sortingLayerName = "Background";

            newObject.SetActive(false);

            newItem.GO = newObject;

            // build boundaries that we will be able to see the
            // planet

            // For now multiply camera rect by distance
            // then when in range object will move 1 / distance
            Bounds Bound = new Bounds();

            Bounds CamBound = CameraExtensions.MaxOrthographicBounds(GetComponent<Camera>());

            Bound.SetMinMax((CamBound.min * newItem.Distance),
                (CamBound.max * newItem.Distance));

            Bound.center = new Vector3(newItem.X, newItem.Y, 0);

            newItem.Bound = Bound;

            _scrollItems.Add(newItem);
        }

        public void AddScrollObjects(List<ParellaxItem> newItems)
        {
            if (_scrollItems == null)
            {
                _scrollItems = new List<ParellaxItem>();
                StartCoroutine("CycleScroller");
            }

            foreach (ParellaxItem newItem in newItems)
                AddScrollObject(newItem);
        }

        #endregion

        #region COROUTINES

        /// <summary>
        /// When the playership is active, detect if the 
        /// </summary>
        /// <returns></returns>
        private IEnumerator CycleScroller()
        {
            while(true)
            {
                GameObject player = GameObject.FindGameObjectWithTag("PlayerShip");
                if(player != null)
                {
                    foreach (ParellaxItem pItem in _scrollItems)
                    {
                        if (pItem.Bound.Contains(player.transform.position))
                        {
                            if(!pItem.GO.activeSelf)
                                pItem.GO.SetActive(true);

                            Bounds CamBound = CameraExtensions.MaxOrthographicBounds(GetComponent<Camera>());

                            Vector3 LocalPos = new Vector3();

                            float leftPos = player.transform.position.x  - pItem.Bound.center.x;
                            float leftPerc = leftPos / pItem.Bound.extents.x;

                            float rightPos = -(CamBound.extents.x * leftPerc);

                            float topPos = player.transform.position.y - pItem.Bound.center.y;
                            float topPerc = topPos / pItem.Bound.extents.y;
                            float bottomPos = -(CamBound.extents.y * topPerc);

                            LocalPos.x = rightPos;
                            LocalPos.y = bottomPos;
                            LocalPos.z = 10;

                            pItem.GO.transform.localPosition = LocalPos;
                        }
                        else
                        {
                            if (pItem.GO.activeSelf)
                                pItem.GO.SetActive(false);
                        }

                        yield return null;
                    }
                }
                
                yield return null;
            }
        }

        #endregion
    }
}