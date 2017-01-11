using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class EditorZoomRect : MonoBehaviour, IPointerDownHandler
{
    public float zoomLevel;

    public Vector2 origScale;

    bool DblClick = false;

    void Awake()
    {
        zoomLevel = 1;
        origScale = transform.localScale;
    }

    void Update()
    {
        if(Input.mouseScrollDelta.y != 0)
        {
            if(Input.mouseScrollDelta.y > 0)
                ZoomIn();
            else
                ZoomOut();
        }
    }

    public void OnPointerDown(PointerEventData data)
    {
        // when middle button is pressed - track delta and drag rect
        if (data.button == PointerEventData.InputButton.Middle)
        {
            // player successfully executed a double click
            if(DblClick)
            {
                ZoomReset();
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
    
    public void ZoomIn()
    {
        zoomLevel += .1f;
        if (zoomLevel > 2)
            zoomLevel = 2;
        
        ApplyZoom();
    }
    
    public void ZoomOut()
    {
        zoomLevel -= .1f;
        if (zoomLevel < .5f)
            zoomLevel = .5f;
        
        ApplyZoom();
    }
    
    public void ZoomReset()
    {
        zoomLevel = 1f;
        ApplyZoom();
    }
    
    public void ApplyZoom()
    {
        transform.localScale = origScale * zoomLevel;
    }
}

