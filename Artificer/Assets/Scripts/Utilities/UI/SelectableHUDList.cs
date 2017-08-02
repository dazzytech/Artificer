using Space.Ship;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// acts as a template and automation for
    /// HUDPanels that manage lists of selectable HUD
    /// items, also shared interaction
    /// </summary>
    public abstract class SelectableHUDList : HUDPanel
    {
        #region ATTRIBUTES

        /// <summary>
        /// Stores the list of prefabs in a singular place
        /// </summary>
        protected List<SelectableHUDItem> m_prefabList
            = new List<SelectableHUDItem>();

        #endregion

        #region PUBLIC INTERACTION

        public virtual void SelectItem(int ID, bool multiSelect)
        {
            if (!multiSelect)
                ClearAll();

            SelectableHUDItem item = m_prefabList.FirstOrDefault
                (x => x.SharedIndex == ID);

            if (item != null)
                item.Select();
        }

        public virtual void HoverItem(int ID, bool multiSelect)
        {
            if (!multiSelect)
                ClearAll();

            SelectableHUDItem item = m_prefabList.FirstOrDefault
                (x => x.SharedIndex == ID);

            if (item != null)
                item.Highlight(true);
        }

        public virtual void LeaveItem(int ID)
        {
            SelectableHUDItem item = m_prefabList.FirstOrDefault
                (x => x.SharedIndex == ID);

            if (item != null)
                item.Highlight(false);
        }

        public virtual void DeselectItem(int ID)
        {
            SelectableHUDItem item = m_prefabList.FirstOrDefault
                (x => x.SharedIndex == ID);

            if (item != null)
                item.Deselect();
        }

        #endregion

        #region PRIVATE UTILITIES

        protected virtual void ClearAll()
        {
            foreach (SelectableHUDItem item in m_prefabList)
                item.Deselect();
        }

        #endregion
    }
}
