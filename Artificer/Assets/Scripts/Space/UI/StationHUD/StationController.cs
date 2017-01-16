using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Space.Ship;
using Space.Ship.Components.Attributes;
using Space.Ship.Components.Listener;

namespace Space.UI.Station
{
    /// <summary>
    /// Performs tasks for the station HUD
    /// such as initializing views
    /// </summary>
    [RequireComponent(typeof(StationAttributes))]
    public class StationController : MonoBehaviour
    {
        #region ATTRIBUTES

        private StationAttributes m_att;

        #endregion

        #region MONO BEHAVOUR

        // Use this for initialization
        void Awake()
        {
            m_att = GetComponent<StationAttributes>();
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Initializes the station HUD.
        /// for now only pass ship atts
        /// Entry Point
        /// </summary>
        /// <param name="param">Parameter.</param>
        public void InitializeHUD(ShipAttributes ship)
        {
            m_att.Ship = ship;

            BuildComponentPanel();

            m_att.Viewer.BuildShip(ship);
        }

        #endregion

        #region INTERNAL INTERACTION

        private void BuildComponentPanel()
        {
            m_att.Busy = false;

            // Clear previous components
            foreach (Transform child in m_att.SelectionListPanel.transform)
                Destroy(child.gameObject);

            // Reset scroller positoning
            Vector2 newPos = new Vector2
                (m_att.SelectionListPanel.transform.localPosition.x, 0f);

            //m_att.SelectionListScroll.value = 0;

            m_att.SelectionListPanel.transform.localPosition = newPos;

            // loop for each component currently stored
            foreach (ComponentListener comp in m_att.Ship.Components)
            {
                // Create a component list item
                GameObject newCLI = Instantiate(m_att.SelectionListPrefab);

                // Set Parent
                newCLI.transform.SetParent
                    (m_att.SelectionListPanel.transform, false);

                // Retrieve behavior to initialize
                ComponentListItem lItem =
                    newCLI.GetComponent<ComponentListItem>();

                // Now initialise
                lItem.DefineComponent(comp.GetAttributes());
            }
        }

        /// <summary>
        /// Removes selected components and resets color of 
        /// each component item
        /// </summary>
        private void ClearSelection()
        {
            m_att.SelectedIDs.Clear();

            // Get each list item and revert its colour
            foreach (Transform child in m_att.SelectionListPanel.transform)
                child.GetComponent<ComponentListItem>().ResetColor();
        }

        #endregion

        #region COROUTINES

        /// <summary>
        /// Loop through components and slowly heal each
        /// </summary>
        /// <returns></returns>
        public IEnumerator HealComponents()
        {
            m_att.Busy = true;

            foreach (ComponentListener comp in
                m_att.Ship.Components)
            {
                if(m_att.SelectedIDs.Contains
                    (comp.GetAttributes().ID))
                {
                    while (comp.GetAttributes().NormalizedHealth < 1f)
                    {
                        comp.HealComponent(0.3f);

                        yield return null;
                    }
                }

                yield return null;
            }

            m_att.Busy = false;

            ClearSelection();

            StopCoroutine("HealComponents");

            yield return null;
        }

        #endregion
    }
}
