using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Menu
{
    [RequireComponent(typeof(Video_Behaviour))]

    public class Video_EventListener : MonoBehaviour
    {


        // Store Behaviour for changing attributes
        [SerializeField]
        private Video_Behaviour m_controller;

        // Video Controls
        public ResolutionListItem Selected;
        private bool _fullscreen;
    	
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
            if (Selected != null)
                Selected.button.image.color = new Color(0f, 0f, 0f, 0f);

            resItem.button.image.color = new Color(.8f,.2f,.2f,.5f);
            Selected = resItem;
        }

        /// <summary>
        /// Sets the fullscreen.
        /// 1st: using the toggle
        /// 2nd: using bool
        /// </summary>
        public void SetFullscreen()
        {
            _fullscreen = m_controller.Fullscreen.isOn;
        }
        public void SetFullscreen(bool isOn)
        {
            _fullscreen = m_controller.Fullscreen.isOn
                = isOn;
        }

        /// <summary>
        /// Applies the resolution and fullscreen.
        /// </summary>
        public void ApplyResolution()
        {
            // Apply resolution if one is selected
            if (Selected != null)
                Video_Config.Video.SetResolution(Selected.resolution, _fullscreen);
            else
                Video_Config.Video.SetResolution(Screen.currentResolution, _fullscreen);
        }

        /// <summary>
        /// Sets the V sync.
        /// </summary>
        public void IncVSync()
        {
            Video_Config.Video.VSync++;
            m_controller.vSync.text = Video_Config.Video.VSync.ToString();
            m_controller.QualityLabel.text = "Custom";
        }

        public void DecVSync()
        {
            Video_Config.Video.VSync--;
            m_controller.vSync.text = Video_Config.Video.VSync.ToString();
            m_controller.QualityLabel.text = "Custom";
        }

        public void IncAA()
        {
            Video_Config.Video.SetAA = m_controller.aa = m_controller.aa == 0? 2: m_controller.aa == 2? 4: 8;
            m_controller.AA.text = Video_Config.Video.GetAA;
            m_controller.QualityLabel.text = "Custom";
        }
        
        public void DecAA()
        {
            Video_Config.Video.SetAA = m_controller.aa = m_controller.aa == 8? 4: m_controller.aa == 4? 2: 0;
            m_controller.AA.text = Video_Config.Video.GetAA;
            m_controller.QualityLabel.text = "Custom";
        }

        public void IncTex()
        {
            Video_Config.Video.SetTexQual = m_controller.tex = m_controller.tex >= 3? 3: ++m_controller.tex;
            m_controller.TexQual.text = Video_Config.Video.GetTexQual;
            m_controller.QualityLabel.text = "Custom";
        }
        
        public void DecTex()
        {
            Video_Config.Video.SetTexQual = m_controller.tex =  m_controller.tex <= 0? 0: --m_controller.tex;
            m_controller.TexQual.text = Video_Config.Video.GetTexQual;
            m_controller.QualityLabel.text = "Custom";
        }

        public void SetQuality(string Qual)
        {
            Video_Config.Video.Settings = Qual;
            m_controller.QualityLabel.text = Video_Config.Video.Settings;

            m_controller.aa = QualitySettings.antiAliasing;
            m_controller.AA.text = Video_Config.Video.GetAA;
            m_controller.tex = QualitySettings.masterTextureLimit;
            m_controller.TexQual.text = Video_Config.Video.GetTexQual;
            // Toggle VSync
            m_controller.vSync.text = Video_Config.Video.VSync.ToString();
        }
    }
}
