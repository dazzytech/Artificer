using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace Construction.ShipEditor
{
    /// <summary>
    /// Editor scroll rect - utility function that enables the middle mouse button 
    /// to move the rect within the bounds of the parent
    /// </summary>
    public class EditorScrollRect : MonoBehaviour, IPointerDownHandler,
        IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        Vector2 DragDelta = new Vector2();
        Vector2 PrevMPos = new Vector2();
        bool Tracking = false;

        bool DblClick = false;

        public void OnPointerDown(PointerEventData data)
        {
            // when middle button is pressed - track delta and drag rect
            if (data.button == PointerEventData.InputButton.Middle)
            {
                // player successfully executed a double click
                if(DblClick)
                {
                    transform.localPosition = new Vector3(0,0);
                    DblClick = false;
                }
                // set player up for a double click
                DblClick = true;
                Invoke("Cancel", .3f);
            }
        }

        public void Cancel()
        {
            DblClick = false;
        }

        public void OnBeginDrag(PointerEventData data)
        {
            // when middle button is pressed - track delta and drag rect
            if (data.button == PointerEventData.InputButton.Middle)
            {
                PrevMPos = data.position;
                Tracking = true;
            }
        }

        public void OnDrag(PointerEventData data)
        {
            // when middle button is pressed - track delta and drag rect
            if (data.button == PointerEventData.InputButton.Middle)
            {
                DragDelta = (data.position - PrevMPos);
                PrevMPos = data.position;
            }
        }
    
        public void OnEndDrag(PointerEventData data)
        {
            if (data.button == PointerEventData.InputButton.Middle)
            {
                Tracking = false;
                DragDelta = (data.position - PrevMPos);
            }
        }

        void LateUpdate()
        {
            // Update rect position based on delta
            transform.Translate(DragDelta);

            if (!Tracking)
                DragDelta *= 0.88f;
            else
                DragDelta = Vector2.zero;

            // Keep edges within rect of parent
            Vector2 newPos = Vector3.zero;
            float thisLeft = transform.position.x - (GetComponent<RectTransform>().rect.width*.5f);
            float parentLeft = transform.parent.position.x - (transform.parent.GetComponent<RectTransform>().rect.width*.5f);

            float thisRight = transform.position.x + (GetComponent<RectTransform>().rect.width*.5f);
            float parentRight = transform.parent.position.x + (transform.parent.GetComponent<RectTransform>().rect.width*.5f);

            float thisBottom = transform.position.y - (GetComponent<RectTransform>().rect.height*.5f);
            float parentBottom = transform.parent.position.y - (transform.parent.GetComponent<RectTransform>().rect.height*.5f);

            float thisTop = transform.position.y + (GetComponent<RectTransform>().rect.height*.5f);
            float parentTop = transform.parent.position.y + (transform.parent.GetComponent<RectTransform>().rect.height*.5f);

            if (thisLeft > parentLeft)
            {
                newPos.x = -(thisLeft - parentLeft);
            }

            if (thisRight < parentRight)
            {
                newPos.x = -(thisRight - parentRight);
            }

            if (thisBottom > parentBottom)
            {
                newPos.y = -(thisBottom - parentBottom);
            }

            if (thisTop < parentTop)
            {
                newPos.y = -(thisTop - parentTop);
            }

            transform.Translate(newPos);
        }
    }
}

