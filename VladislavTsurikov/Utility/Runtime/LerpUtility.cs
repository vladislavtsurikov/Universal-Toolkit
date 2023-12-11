using System;
using System.Collections;
using UnityEngine;

namespace VladislavTsurikov.Utility.Runtime
{
    public static class LerpUtility
    {
        public static IEnumerator Lerp(float start, float end, float seconds, Action<float> callback, Action onComplete = null)
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