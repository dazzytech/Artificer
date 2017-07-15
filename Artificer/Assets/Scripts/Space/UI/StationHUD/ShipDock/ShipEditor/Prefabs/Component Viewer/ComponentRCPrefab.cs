using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

using Space.Ship.Components.Listener;
using Space.Ship.Components.Attributes;
using Space.UI.Station.Editor;
using Space.UI.Station.Editor.Component;
using Space.UI.Station.Utility;

namespace Space.UI.Station.Prefabs
{
    /// <summary>
    /// Appears when a component is rightclicked
    /// interactable behaviour with dragging addon
    /// </summary>
    public class ComponentRCPrefab : ComponentInteractivePrefab, IDragHandler
    {
        #region POINTER DATA

        public void OnDrag(PointerEventData data)
        {
            // can drag to better position     
            Vector2 currentPosition = transform.position;
            currentPosition.x += data.delta.x;
            currentPosition.y += data.delta.y;
            transform.position = currentPosition;
        }

        #endregion
    }
}