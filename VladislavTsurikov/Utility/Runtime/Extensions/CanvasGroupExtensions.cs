using System.Collections;
using UnityEngine;

namespace VladislavTsurikov.Utility.Runtime.Extensions
{
    public static class CanvasGroupExtensions
    {
        public static IEnumerator Fade(this CanvasGroup group, float to, float seconds, bool setBlocksRaycasts = true)
        {
            if (!group || !group.gameObject.activeInHierarchy)
                yield break;

            if (setBlocksRaycasts)
                group.blocksRaycasts = true;

            if (group.alpha == to)
                yield break;
            
            yield return LerpUtility.Lerp(group.alpha, to, seconds, t =>
            {
                if (group)
                    group.alpha = t;
                
                if (setBlocksRaycasts)
                    group.blocksRaycasts = group.alpha > 0;
            
            });
        }
    }
}