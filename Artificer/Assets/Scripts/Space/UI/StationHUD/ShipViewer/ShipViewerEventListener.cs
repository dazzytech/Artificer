using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Space.Ship.Components.Listener;
using UI;

namespace Space.UI.Station.Viewer
{
    /// <summary>
    /// Listening compoent for player interaction
    /// with 
    /// </summary>
    public class ShipViewerEventListener : MonoBehaviour
    {
        #region ATTRIBUTES

        private ShipViewerAttributes m_att;
        private ShipViewerController m_con;

        #endregion

        #region MONO BEHAVIOUR

        void Awake()
        {
            m_att = GetComponent<ShipViewerAttributes>();
            m_con = GetComponent<ShipViewerController>();
        }

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

        #region VIEWER STATE

        /// <summary>
        /// Called when state is changed
        /// Activates all GOs associated with that state
        /// </summary>
        public void OnStateChanged(int newState)
        {
            switch (newState)
            {
                case 0:
                    m_att.State = ViewerState.MANAGE;
                    m_con.InitializeManage();
                    break;
                case 1:
                    m_att.State = ViewerState.EDIT;
                    m_con.InitializeEdit();
                    break;
            }
            
        }

        #endregion

        public void ExitStation()
        {
            if(!m_att.Busy)
                SystemManager.Space.LeaveStation();
        }

        public void RepairSelected()
        {
            if (m_att.SelectedIDs.Count > 0)
                m_con.StartCoroutine("HealComponents");
        }

        public void RepairAll()
        {
            foreach (ComponentListener comp in
               m_att.Ship.Components)
            {
                if (!m_att.SelectedIDs.Contains
                    (comp.GetAttributes().ID))
                {
                    m_att.SelectedIDs.Add
                        (comp.GetAttributes().ID);

                    m_con.SelectItem(comp.GetAttributes().ID);
                }
            }

            if (m_att.SelectedIDs.Count > 0)
                m_con.StartCoroutine("HealComponents");
        }

        public void ComponentSelected(int ID)
        {
            if (!m_att.SelectedIDs.Contains(ID))
                m_att.SelectedIDs.Add(ID);

            m_con.SelectItem(ID);
        }

        public void ComponentDeselected(int ID)
        {
            if (m_att.SelectedIDs.Contains(ID))
                m_att.SelectedIDs.Remove(ID);

            m_con.ClearItem(ID);
        }

        public void ComponentHover(int ID)
        {
            if (!m_att.SelectedIDs.Contains(ID))
                m_con.HoverItem(ID);
        }

        public void ComponentLeave(int ID)
        {
            if (!m_att.SelectedIDs.Contains(ID))
                m_con.LeaveItem(ID);
        }

        #endregion
    }
}
