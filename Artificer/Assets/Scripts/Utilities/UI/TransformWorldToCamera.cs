using UnityEngine;
using System.Collections;

public class UIConvert
{
    public static Vector3 WorldToCamera(Transform trackedObj)
    {
        //then you calculate the position of the UI element
        //0,0 for the canvas is at the center of the screen, whereas WorldToViewPortPoint treats the lower left corner as 0,0. Because of this, you need to subtract the height / width of the canvas * 0.5 to get the correct position.
        RectTransform CameraRect = GameObject.Find("_gui").GetComponent<RectTransform>();

        Vector2 ViewportPosition=Camera.main.WorldToViewportPoint(trackedObj.position);
        Vector3 WorldObject_ScreenPosition=new Vector3(
            ((ViewportPosition.x*CameraRect.sizeDelta.x)-(CameraRect.sizeDelta.x*0.5f)),
            ((ViewportPosition.y*CameraRect.sizeDelta.y)-(CameraRect.sizeDelta.y*0.5f)), 0f);

        return WorldObject_ScreenPosition;
    }

    public static Vector3 WorldToCameraPoint(Vector3 point)
    {
        //then you calculate the position of the UI element
        //0,0 for the canvas is at the center of the screen, whereas WorldToViewPortPoint treats the lower left corner as 0,0. Because of this, you need to subtract the height / width of the canvas * 0.5 to get the correct position.
        RectTransform CameraRect = GameObject.Find("_gui").GetComponent<RectTransform>();

        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(point);
        Vector2 WorldObject_ScreenPosition = new Vector2(
            ((ViewportPosition.x * CameraRect.sizeDelta.x) - (CameraRect.sizeDelta.x * 0.5f)),
            ((ViewportPosition.y * CameraRect.sizeDelta.y) - (CameraRect.sizeDelta.y * 0.5f)));

        return WorldObject_ScreenPosition;
    }

    public static Vector2 WorldToCameraRect(Rect rect)
    {
        //then you calculate the position of the UI element
        //0,0 for the canvas is at the center of the screen, whereas WorldToViewPortPoint treats the lower left corner as 0,0. Because of this, you need to subtract the height / width of the canvas * 0.5 to get the correct position.
        RectTransform CameraRect = GameObject.Find("_gui").GetComponent<RectTransform>();
        
        Vector2 ViewportPosition=Camera.main.WorldToViewportPoint(rect.position);
        Vector2 WorldObject_ScreenPosition=new Vector2(
            ((ViewportPosition.x*CameraRect.sizeDelta.x)-(CameraRect.sizeDelta.x*0.5f)),
            ((ViewportPosition.y*CameraRect.sizeDelta.y)-(CameraRect.sizeDelta.y*0.5f)));
        
        return WorldObject_ScreenPosition;
    }

    public static Vector2 WorldToCameraRectEnd(Rect rect)
    {
        //then you calculate the position of the UI element
        //0,0 for the canvas is at the center of the screen, whereas WorldToViewPortPoint treats the lower left corner as 0,0. Because of this, you need to subtract the height / width of the canvas * 0.5 to get the correct position.
        RectTransform CameraRect = GameObject.Find("_gui").GetComponent<RectTransform>();
        
        Vector2 ViewportPosition=Camera.main.WorldToViewportPoint(rect.position + rect.size);
        Vector2 WorldObject_ScreenPosition=new Vector2(
            ((ViewportPosition.x*CameraRect.sizeDelta.x)-(CameraRect.sizeDelta.x*0.5f)),
            ((ViewportPosition.y*CameraRect.sizeDelta.y)-(CameraRect.sizeDelta.y*0.5f)));
        
        return WorldObject_ScreenPosition;
    }
}

