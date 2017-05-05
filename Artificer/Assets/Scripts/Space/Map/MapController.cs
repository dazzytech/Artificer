using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Space.Map
{
    /// <summary>
    /// Updates map elements list
    /// in a central area
    /// </summary>
    [RequireComponent(typeof(MapAttributes))]
    public class MapController : MonoBehaviour
    {
        #region EVENTS

        public delegate void MapUpdate(MapObject obj);

        public static event MapUpdate OnMapUpdate;

        #endregion

        #region ATTRIBUTES

        [SerializeField]
        private MapAttributes m_att;

        #endregion

        #region ACCESSORS

        public List<MapObject> Map
        {
            get { return m_att.MapItems; }
        }

        #endregion

        #region PUBLIC INTERACTION

        // Use this for initialization
        public void InitializeMap()
        {
            StartCoroutine("BuildMap");
            StartCoroutine("UpdateList");
        }

        #endregion

        #region PRIVATE UTILITIES

        private void BuildObject(Transform child, int i)
        {
            MapObject mapObj = m_att.MapItems.FirstOrDefault
                                (x => x.Ref == child);

            if (mapObj == null)
            {
                // object not already added
                mapObj = new MapObject();
                mapObj.Location = child.position;
                mapObj.Ref = child;
                mapObj.Type = (MapObjectType)i;

                m_att.MapItems.Add(mapObj);

                if(OnMapUpdate != null)
                    OnMapUpdate(mapObj);
            }
        }

        #endregion

        #region COROUTINES

        /// <summary>
        /// Loops through each item in the game
        /// Scene and creates a map item and stores it
        /// if not already stored
        /// </summary>
        /// <returns></returns>
        private IEnumerator BuildMap()
        {
            // forever loop
            while (true)
            {
                int i = 0;

                // loop through each sub object for physical items
                foreach (string category in m_att.SearchItems)
                {
                    // behave differently if teams
                    if (category == "_teams")
                    {
                        // retrieve object
                        Transform topContainer =
                            SystemManager.Space.transform.Find(category);

                        // retrieve object
                        Transform container =
                            topContainer.GetChild(0);

                        // Build a map object for each transform
                        foreach (Transform child in container)
                        {
                            BuildObject(child, i);

                            yield return null;
                        }

                        // retrieve object
                        container =
                            topContainer.GetChild(1);

                        // Build a map object for each transform
                        foreach (Transform child in container.transform)
                        {
                            BuildObject(child, i + 1);

                            yield return null;
                        }
                    }
                    else
                    {
                        // retrieve object
                        Transform container =
                            SystemManager.Space.transform.Find(category);

                        // Build a map object for each transform
                        foreach (Transform child in container)
                        {
                            BuildObject(child, i);

                            yield return null;
                        }

                        i++;
                    }

                    yield return null;
                }
                yield return null;
            }
        }

        /// <summary>
        /// Loops through each map
        /// item and update current state
        /// </summary>
        /// <returns></returns>
        private IEnumerator UpdateList()
        {
            while (true)
            {
                for(int i = 0; i < m_att.MapItems.Count; i++)
                {
                    MapObject mObj = m_att.MapItems[i];

                    // Remove from list if transform is null
                    // e.g. object destroyed
                    if (mObj.Ref == null)
                        m_att.MapItems.RemoveAt(i--); // will dec after completed

                    else if (mObj.Location.x != mObj.Ref.position.x
                        || mObj.Location.y != mObj.Ref.position.y)
                        // update location
                        mObj.Location = mObj.Ref.position;
                    else
                        continue;

                    // Update any viewers
                    if (OnMapUpdate != null)
                        OnMapUpdate(mObj);
                }

                yield return null;
            }
        }

        #endregion
    }
}
