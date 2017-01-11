using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HintBoxController : MonoBehaviour
{
    public static Text message;

    void Start()
    {
        message = GetComponent<Text>();
    }

    public static void Display(string text)
    {
        message.text = text;
    }

    public static void Clear()
    {
        message.text = "";
    }

    public static void Clear(string text)
    {
        if(message.text == text)
            message.text = "";
    }
}

