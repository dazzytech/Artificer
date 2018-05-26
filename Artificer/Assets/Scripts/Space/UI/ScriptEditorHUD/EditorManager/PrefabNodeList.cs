using Data.UI;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Space.UI.IDE
{
    /// <summary>
    /// Stores lists and categories of nodes that may be 
    /// added to script
    /// </summary>
    public class PrefabNodeList : MonoBehaviour
    {
        #region ATTIBUTES

        [SerializeField]
        private EditorManager m_con;

        [Header("UI Prefabs")]

        [SerializeField]
        private GameObject m_panelPrefabs;

        [SerializeField]
        private GameObject m_nodeCreatePrefab;

        [SerializeField]
        private GameObject m_tabPrefab;

        [Header("UI Elements")]

        
        [SerializeField]
        private Transform m_tabList;

        /// <summary>
        /// Holds each HUD list for each category
        /// </summary>
        [SerializeField]
        private Transform m_listContainer;

        /// <summary>
        /// uses the catergory as an index when
        /// opening the prefab list
        /// </summary>
        private Dictionary<string, Transform> m_prefabLists
            = new Dictionary<string, Transform>();

        #endregion

        #region PUBLIC INTERACTION
        
        /// <summary>
        /// Generates the list of prefabs
        /// </summary>
        /// <param name="nodeCreate"></param>
        public void GeneratePrefabs()
        {
            foreach (Transform child in m_listContainer)
                GameObject.Destroy(child);

            foreach (string category in m_con.Prefabs.Categories)
            {
                // build the panel and populate
                GameObject panel = Instantiate(m_panelPrefabs);
                panel.transform.SetParent(m_listContainer);
                

                // only do the one category
                foreach (NodeData node in m_con.Prefabs.GetCategory(category))
                {
                    GameObject nodeGO = Instantiate(m_nodeCreatePrefab);
                    nodeGO.transform.SetParent(panel.transform, false);

                    nodeGO.GetComponent<NodeCreatePrefab>()
                        .CreateItem(node, new EditorManager.Create(m_con.AddNode));
                }

                // Build the tab button and assign the panel to the tab
                GameObject tab = Instantiate(m_tabPrefab);
                tab.transform.SetParent(m_tabList);
                tab.GetComponentInChildren<Text>().text = category;
                tab.GetComponentInChildren<Button>().onClick.AddListener
                    (delegate { OpenTransform(category); });

                m_prefabLists.Add(category, panel.transform);
            }
        }

        /// <summary>
        /// Close all category lists and open the list selected
        /// </summary>
        /// <param name="category"></param>
        public void OpenTransform(string category)
        {
            foreach (Transform panels in m_prefabLists.Values)
                panels.gameObject.SetActive(false);

            m_prefabLists[category].gameObject.SetActive(true);
            m_prefabLists[category].localPosition = new Vector3(0, 0, 0);
        }

        #endregion
    }
}
