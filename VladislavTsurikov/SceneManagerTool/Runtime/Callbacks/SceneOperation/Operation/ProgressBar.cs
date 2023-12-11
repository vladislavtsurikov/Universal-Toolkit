using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneCollectionSystem;
using VladislavTsurikov.Utility.Runtime.Extensions;

namespace VladislavTsurikov.SceneManagerTool.Runtime.Callbacks.SceneOperation.Operation
{
    public class ProgressBar : SceneOperation
    {
        private SettingsSystem.Components.ProgressBar _progressBar;
        
        public CanvasGroup Group;
        public Image Image;
        public Slider Slider;
        public float Duration = 0.5f;
        public Color Color;

        public override IEnumerator OnLoad()
        {
            if(_progressBar.DisableFade)
                yield break;
            
            Image.color = Color;
            yield return Group.Fade(1, Duration); 
        }

        public override IEnumerator OnUnload()
        {
            if(_progressBar.DisableFade)
                yield break;
            
            if (Slider)
                Slider.gameObject.SetActive(false);
            yield return Group.Fade(0, Duration);
        }

        private void Start()
        {
            _progressBar = (SettingsSystem.Components.ProgressBar)SceneCollection.Current.SettingsList.GetElement(typeof(SettingsSystem.Components.ProgressBar));
            
            if(!_progressBar.DisableFade)
                Group.alpha = 0;
        }

        private void Update()
        {
            if (Slider)
                Slider.value = SceneCollection.Current.LoadingProgress;
        }
    }
}
