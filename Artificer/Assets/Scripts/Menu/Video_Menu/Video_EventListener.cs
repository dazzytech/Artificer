using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Menu
{
    [RequireComponent(typeof(Video_Behaviour))]

    public class Video_EventListener : MonoBehaviour
    {
        // Store Behaviour for changing attributes
        private Video_Behaviour _controller;

        // Video Controls
        private ResolutionListItem _selected;
        private bool _fullscreen;

        // Use this for initialization
        void Awake()
        {
            _controller = GetComponent<Video_Behaviour>();
        }
    	
        // Update is called once per frame
        void Update()
        {
    	
        }

        /// <summary>
        /// Selects the resolution.
        /// highlight button
        /// </summary>
        /// <param name="resItem">Res item.</param>
        public void SelectResolution(ResolutionListItem resItem)
        {
            if (_selected != null)
                _selected.button.image.color = new Color(0f, 0f, 0f, 0f);

            resItem.button.image.color = new Color(.8f,.2f,.2f,.5f);
            _selected = resItem;
        }

        /// <summary>
        /// Sets the fullscreen.
        /// 1st: using the toggle
        /// 2nd: using bool
        /// </summary>
        public void SetFullscreen()
        {
            _fullscreen = _controller.Fullscreen.isOn;
        }
        public void SetFullscreen(bool isOn)
        {
            _fullscreen = _controller.Fullscreen.isOn
                = isOn;
        }

        /// <summary>
        /// Applies the resolution and fullscreen.
        /// </summary>
        public void ApplyResolution()
        {
            // Apply resolution if one is selected
            if (_selected != null)
                Video_Config.Video.SetResolution(_selected.resolution, _fullscreen);
            else
                Video_Config.Video.SetResolution(Screen.currentResolution, _fullscreen);
        }

        /// <summary>
        /// Sets the V sync.
        /// </summary>
        public void IncVSync()
        {
            Video_Config.Video.VSync++;
            _controller.vSync.text = Video_Config.Video.VSync.ToString();
            _controller.QualityLabel.text = "Custom";
        }

        public void DecVSync()
        {
            Video_Config.Video.VSync--;
            _controller.vSync.text = Video_Config.Video.VSync.ToString();
            _controller.QualityLabel.text = "Custom";
        }

        public void IncAA()
        {
            Video_Config.Video.SetAA = _controller.aa = _controller.aa == 0? 2: _controller.aa == 2? 4: 8;
            _controller.AA.text = Video_Config.Video.GetAA;
            _controller.QualityLabel.text = "Custom";
        }
        
        public void DecAA()
        {
            Video_Config.Video.SetAA = _controller.aa = _controller.aa == 8? 4: _controller.aa == 4? 2: 0;
            _controller.AA.text = Video_Config.Video.GetAA;
            _controller.QualityLabel.text = "Custom";
        }

        public void IncTex()
        {
            Video_Config.Video.SetTexQual = _controller.tex = _controller.tex >= 3? 3: ++_controller.tex;
            _controller.TexQual.text = Video_Config.Video.GetTexQual;
            _controller.QualityLabel.text = "Custom";
        }
        
        public void DecTex()
        {
            Video_Config.Video.SetTexQual = _controller.tex =  _controller.tex <= 0? 0: --_controller.tex;
            _controller.TexQual.text = Video_Config.Video.GetTexQual;
            _controller.QualityLabel.text = "Custom";
        }

        public void SetQuality(string Qual)
        {
            Video_Config.Video.Settings = Qual;
            _controller.QualityLabel.text = Video_Config.Video.Settings;

            _controller.aa = QualitySettings.antiAliasing;
            _controller.AA.text = Video_Config.Video.GetAA;
            _controller.tex = QualitySettings.masterTextureLimit;
            _controller.TexQual.text = Video_Config.Video.GetTexQual;
            // Toggle VSync
            _controller.vSync.text = Video_Config.Video.VSync.ToString();
        }
    }
}
