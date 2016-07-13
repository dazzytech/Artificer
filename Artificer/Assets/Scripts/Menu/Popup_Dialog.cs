using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Menu
{
    // Enumerators
    //dialog result 
    public enum DialogResult{NULL = 0,
        OK, CANCEL, YES, NO}
    
    public enum DialogType{OK = 0,
        YESNO, YESNOTIMER, LOADDIALOG};

    [RequireComponent(typeof(Menu_Behaviour))]

    public class Popup_Dialog : MonoBehaviour
    {
        // reference to the menu attributes
        private static Menu_Behaviour _controller;
        private static MenuState _prev;

        static public Popup_Dialog instance;

        // Event system
        public delegate void DialogEvent (DialogResult returnResult);
        public static DialogEvent OnDialogEvent;

        // for now only have a yesnoframe
        public GameObject YesNoFrame;

        public static float _timer;
        public static float _dTimer;
        public static bool _timerRunning;

        void Awake()
        {
            instance = this;
            _controller = GetComponent<Menu_Behaviour>();
        }

        public static void ShowPopup
            (string head, string body, DialogType type)
        {
            _prev = _controller.CurrentState;
            _controller.CurrentState = MenuState.Popup;
            switch (type)
            {
                case DialogType.YESNOTIMER:
                    _dTimer = 0;
                    _timer = 10.0f;
                    instance.YesNoFrame.SetActive(true);
                    instance.YesNoFrame.transform.Find("Header").GetComponent<Text>().text = head;
                    instance.YesNoFrame.transform.Find("Body").GetComponent<Text>().text = body;
                    _timerRunning = true;
                    break;
                case DialogType.YESNO:
                    instance.YesNoFrame.SetActive(true);
                    instance.YesNoFrame.transform.Find("Header").GetComponent<Text>().text = head;
                    instance.YesNoFrame.transform.Find("Body").GetComponent<Text>().text = body;
                    _timerRunning = false;
                    break;
            }
        }

        void Update()
        {
            if (_timerRunning)
            {
                _dTimer += Time.deltaTime;
                instance.YesNoFrame.transform.Find("Timer").GetComponent<Text>().text =
                    (_timer - _dTimer).ToString("F0");

                if(_dTimer > _timer)
                {
                    if(OnDialogEvent != null)
                    {
                        _controller.CurrentState = _prev;
                        OnDialogEvent(DialogResult.NULL);
                    }
                }
            }
        }

        public void ButtonPress(string input)
        {
            _controller.CurrentState = _prev;

            switch (input)
            {
                case "yes":
                    OnDialogEvent(DialogResult.YES);
                    break;
                case "no":
                    OnDialogEvent(DialogResult.NO);
                    break;
                case "cancel":
                    OnDialogEvent(DialogResult.CANCEL);
                    break;
                case "ok":
                    OnDialogEvent(DialogResult.OK);
                    break;
            }
        }
    }
}
