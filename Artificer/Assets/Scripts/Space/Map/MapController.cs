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
        #region ATTRIBUTES

        [SerializeField]
        private MapAttributes m_att;

        #endregion

        #region PUBLIC INTERACTION

        // Use this for initialization
        public void InitializeMap()
        {
            StartCoroutine("BuildMap");
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
                foreach(string category in m_att.SearchItems)
                {
                    // behave differently if teams
                    if(category == "_teams")
                    {
                        // retrieve object
                        Transform topContainer =
                            SystemManager.Space.transform.Find(category);

                        // retrieve object
                        Transform container =
                            topContainer.GetChild(0);

                        // Build a map object for each transform
                        foreach (Transform child in container.transform)
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
                            BuildObject(child, i+1);

                            yield return null;
                        }
                    }
                    else
                    {
                        // retrieve object
                        Transform container = 
                            SystemManager.Space.transform.Find(category);

                        // Build a map object for each transform
                        foreach(Transform child in container.transform)
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

        #endregion
    }
}
