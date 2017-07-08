using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Space.Ship.Components.Listener;
using UI;

namespace Space.UI.Station.Viewer
{
    /// <summary>
    /// Listening compoent for player interaction
    /// e.g. state changes and button presses
    /// with 
    /// </summary>
    public class ShipViewerEventListener : MonoBehaviour
    {
        #region ATTRIBUTES

        [SerializeField]
        private ShipViewer m_viewer;

        #endregion

        #region MONO BEHAVIOUR

        void OnEnable()
        {
            // listen to component item events
            ComponentListItem.ItemSelected += ComponentSelected;
            ComponentListItem.ItemDeselected += ComponentDeselected;
            ViewerItem.ItemSelected += ComponentSelected;
            ViewerItem.ItemDeselected += ComponentDeselected;


            ComponentListItem.ItemHover += ComponentHover;
            ComponentListItem.ItemLeave += ComponentLeave;
            ViewerItem.ItemHover += ComponentHover;
            ViewerItem.ItemLeave += ComponentLeave;
        }

        void OnDisable()
        {
            // listen to component item events
            ComponentListItem.ItemSelected -= ComponentSelected;
            ComponentListItem.ItemDeselected -= ComponentDeselected;
            ViewerItem.ItemSelected -= ComponentSelected;
            ViewerItem.ItemDeselected -= ComponentDeselected;

            ComponentListItem.ItemHover -= ComponentHover;
            ComponentListItem.ItemLeave -= ComponentLeave;
            ViewerItem.ItemHover -= ComponentHover;
            ViewerItem.ItemLeave -= ComponentLeave;
        }

        #endregion

        #region EVENTS

        public void ComponentSelected(int ID)
        {
            if (!m_viewer.SelectedIDs.Contains(ID))
                m_viewer.SelectedIDs.Add(ID);

            m_viewer.SelectItem(ID);
        }

        public void ComponentDeselected(int ID)
        {
            if (m_viewer.SelectedIDs.Contains(ID))
                m_viewer.SelectedIDs.Remove(ID);

            m_viewer.ClearItem(ID);
        }

        public void ComponentHover(int ID)
        {
            if (!m_viewer.SelectedIDs.Contains(ID))
                m_viewer.HoverItem(ID);
        }

        public void ComponentLeave(int ID)
        {
            if (!m_viewer.SelectedIDs.Contains(ID))
                m_viewer.LeaveItem(ID);
        }

        public void RepairSelected()
        {
            if (m_viewer.SelectedIDs.Count > 0)
                m_viewer.StartCoroutine("HealComponents");
        }

        public void RepairAll()
        {
            foreach (ComponentListener comp in
               m_viewer.Ship.Components)
            {
                if (!m_viewer.SelectedIDs.Contains
                    (comp.GetAttributes().ID))
                {
                    m_viewer.SelectedIDs.Add
                        (comp.GetAttributes().ID);
                }
            }

            if (m_viewer.SelectedIDs.Count > 0)
                m_viewer.StartCoroutine("HealComponents");
        }

        #endregion
    }
}
