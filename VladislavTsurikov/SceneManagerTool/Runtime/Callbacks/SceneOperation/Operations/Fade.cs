using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using VladislavTsurikov.Coroutines.Runtime;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.SceneManagerTool.Runtime.Callbacks.SceneOperation
{
    public class Fade : SceneOperation
    {
        public CanvasGroup Group;
        public Image Image;
        public float Seconds = 1f;
        public Color Color = Color.black;

        public override IEnumerator OnLoad()
        {
            Image.color = Color;
            yield return Group.Fade(1, Seconds);
        }

        public override IEnumerator OnUnload()
        {
            yield return Group.Fade(0, Seconds);
        }

        private void Start()
        {
            Group.alpha = 0;

            CoroutineRunner.StartCoroutine(OnLoad());
        }
    }
}