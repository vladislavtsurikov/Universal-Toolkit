using System;
using System.Collections;
using UnityEngine;

namespace VladislavTsurikov.UnityUtility.Runtime
{
    public static class CanvasGroupExtensions
    {
        public static IEnumerator Fade(this CanvasGroup group, float to, float seconds, bool setBlocksRaycasts = true)
        {
            if (!group || !group.gameObject.activeInHierarchy)
            {
                yield break;
            }

            if (setBlocksRaycasts)
            {
                group.blocksRaycasts = true;
            }

            if (group.alpha == to)
            {
                yield break;
            }
            
            yield return Lerp(group.alpha, to, seconds, t =>
            {
                if (group)
                    group.alpha = t;
                
                if (setBlocksRaycasts)
                    group.blocksRaycasts = group.alpha > 0;
            
            });
        }
        
        private static IEnumerator Lerp(float start, float end, float seconds, Action<float> callback, Action onComplete = null)
        {
            float waitForSeconds = seconds;
            float timeLeft = seconds;

            while (timeLeft >= 0)
            {
                float t = Mathf.InverseLerp(waitForSeconds, 0, timeLeft);
                callback?.Invoke(Mathf.Lerp(start, end, t));

                timeLeft -= Time.deltaTime;
                
                yield return null;
            }

            callback?.Invoke(end);
            onComplete?.Invoke();
        }
    }
}