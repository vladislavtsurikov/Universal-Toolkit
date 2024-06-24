using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneCollectionSystem;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.SceneManagerTool.Runtime.Callbacks.SceneOperation
{
    public class ProgressBar : SceneOperation
    {
        private SettingsSystem.ProgressBar _progressBar;
        
        public CanvasGroup Group;
        public Image Image;
        public Slider Slider;
        public float Duration = 0.5f;
        public Color Color;

        public override IEnumerator OnLoad()
        {
            if(_progressBar.DisableFade)
            {
                yield break;
            }

            Image.color = Color;
            yield return Group.Fade(1, Duration); 
        }

        public override IEnumerator OnUnload()
        {
            if(_progressBar.DisableFade)
            {
                yield break;
            }

            if (Slider)
            {
                Slider.gameObject.SetActive(false);
            }

            yield return Group.Fade(0, Duration);
        }

        private void Start()
        {
            _progressBar = (SettingsSystem.ProgressBar)SceneCollection.Current.SettingsStack.GetElement(typeof(SettingsSystem.ProgressBar));

            if (!_progressBar.DisableFade)
            {
                Group.alpha = 0;
            }
        }

        private void Update()
        {
            if (Slider)
            {
                Slider.value = SceneCollection.Current.LoadingProgress;
            }
        }
    }
}
