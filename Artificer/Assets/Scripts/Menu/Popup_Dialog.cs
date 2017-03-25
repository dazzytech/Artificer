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
        YESNO, YESNOTIMER, LOADDIALOG, INPUTFIELD, IMAGE};

    [RequireComponent(typeof(Menu_Behaviour))]

    public class Popup_Dialog : MonoBehaviour
    {
        #region ATTRIBUTES

        // Reference menu controller
        private static Menu_Behaviour _controller;

        // Stores what state we were previously
        private static MenuState _prev;

        // Static reference to self
        static public Popup_Dialog instance;

        // Timer variables
        public static float _timer;
        public static float _dTimer;
        public static bool _timerRunning;

        #region HUD ELEMENTS 

        [Header("POPUP HUD Elements")]

        #region YES NO FRAME 

        [Header("Yes/No Frame")]

        [SerializeField]
        private GameObject m_yesNoFrame;
        [SerializeField]
        private Text m_yesNoTimer;
        [SerializeField]
        private Text m_yesNoHeader;
        [SerializeField]
        private Text m_yesNoBody;

        #endregion

        #region TEXT INPUT FRAME

        [Header("Input Frame")]

        //Create Text Input Frame
        [SerializeField]
        private GameObject m_inputFrame;
        [SerializeField]
        private InputField m_input;
        [SerializeField]
        private Text m_inputHeader;
        [SerializeField]
        private Text m_inputBody;

        private string m_defaultTxt;

        #endregion

        #region IMAGE FRAME

        [Header("Input Frame")]

        //Create Text Input Frame
        [SerializeField]
        private GameObject m_imageFrame;
        [SerializeField]
        private Image m_image;
        [SerializeField]
        private Text m_imageHeader;
        [SerializeField]
        private Text m_imageBody;
        [SerializeField]
        private GameObject m_imageControl;

        #endregion

        #endregion

        #endregion

        #region EVENTS 

        /// <summary>
        /// Contains a result and an optional parameter
        /// </summary>
        /// <param name="returnResult"></param>
        /// <param name="returnParam"></param>
        public delegate void DialogEvent (DialogResult returnResult,
            object returnParam = null);
        public static DialogEvent OnDialogEvent;

        #endregion

        void Awake()
        {
            instance = this;
            _controller = GetComponent<Menu_Behaviour>();
        }

        public static void ShowPopup
            (string head, string body, DialogType type, 
            object param = null)
        {
            _prev = _controller.CurrentState;
            _controller.CurrentState = MenuState.Popup;

            // deactivate all windows first
            instance.m_yesNoFrame.SetActive(false);
            instance.m_inputFrame.SetActive(false);
            instance.m_imageFrame.SetActive(false);

            switch (type)
            {
                case DialogType.YESNOTIMER:
                    _dTimer = 0;
                    _timer = 10.0f;
                    instance.m_yesNoFrame.SetActive(true);
                    instance.m_yesNoHeader.text = head;
                    instance.m_yesNoBody.text = body;
                    _timerRunning = true;
                    break;
                case DialogType.YESNO:
                    instance.m_yesNoFrame.SetActive(true);
                    instance.m_yesNoHeader.text = head;
                    instance.m_yesNoBody.text = body;
                    _timerRunning = false;
                    break;
                case DialogType.INPUTFIELD:
                    instance.m_inputFrame.SetActive(true);
                    instance.m_inputHeader.text = head;
                    instance.m_inputBody.text = body;
                    // assign default text and HUD element
                    instance.m_input.text =
                        instance.m_defaultTxt = param.ToString();
                    _timerRunning = false;
                    break;
                case DialogType.IMAGE:
                    instance.m_imageFrame.SetActive(true);
                    instance.m_imageHeader.text = head;
                    instance.m_imageBody.text = body;
                    if ((bool)param)
                        instance.m_imageControl.SetActive(true);
                    else
                        instance.m_imageControl.SetActive(false);
                    break;
            }
        }

        /// <summary>
        /// Used if we want to quit out of a popup
        /// pre-maturely
        /// </summary>
        public static void KillPopup()
        {
            if (OnDialogEvent != null)
            {
                _controller.CurrentState = _prev;
                OnDialogEvent(DialogResult.NULL);
            }
        }

        void Update()
        {
            if (_timerRunning)
            {
                _dTimer += Time.deltaTime;
                instance.m_yesNoTimer.GetComponent<Text>().text =
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
                    if(OnDialogEvent != null)
                        OnDialogEvent(DialogResult.OK);
                    break;
                case "submit":
                    if (m_input.text != "")
                        OnDialogEvent(DialogResult.OK, 
                            m_input.text);
                    else
                        OnDialogEvent(DialogResult.OK, 
                            m_defaultTxt);
                    break;
            }
        }
    }
}
