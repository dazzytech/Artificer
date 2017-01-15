using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Space.Ship.Components.Listener;

namespace Space.UI.Station
{
    /// <summary>
    /// Listening compoent for player interaction
    /// with 
    /// </summary>
    [RequireComponent(typeof(StationAttributes))]
    public class StationEventListener : MonoBehaviour
    {
        #region ATTRIBUTES

        private StationAttributes m_att;
        private StationController m_con;

        #endregion

        #region MONO BEHAVIOUR

        void Start()
        {
            m_att = GetComponent<StationAttributes>();
            m_con = GetComponent<StationController>();
        }

        void OnEnable()
        {
            // listen to component item events
            ComponentListItem.ItemSelected += ComponentSelected;
            ComponentListItem.ItemDeselected += ComponentDeselected;
        }

        void OnDisable()
        {
            // listen to component item events
            ComponentListItem.ItemSelected -= ComponentSelected;
            ComponentListItem.ItemDeselected -= ComponentDeselected;
        }

        #endregion

        #region EVENTS

        public void ExitStation()
        {
            if(!m_att.Busy)
                GameManager.Space.LeaveStation();
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
                }
            }

            if (m_att.SelectedIDs.Count > 0)
                m_con.StartCoroutine("HealComponents");
        }

        public void ShipEditor()
        {

        }

        public void ComponentSelected(int ID)
        {
            if (!m_att.SelectedIDs.Contains(ID))
                m_att.SelectedIDs.Add(ID);
        }

        public void ComponentDeselected(int ID)
        {
            if (m_att.SelectedIDs.Contains(ID))
                m_att.SelectedIDs.Remove(ID);
        }

        #endregion
    }
}
