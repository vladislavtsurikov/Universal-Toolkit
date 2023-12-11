using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem.Components
{
    [MenuItem("Failure Rate")]  
    public class FailureRate : Scatter
    {
        public float Value = 70;
        
        public override IEnumerator Samples(BoxArea boxArea, List<Vector2> samples, Action<Vector2> onSpawn = null)
        {
            for (int i = samples.Count - 1; i >= 0 ; i--)
            {
                if (IsWaitForNextFrame())
                {
                    yield return null;
                }
                
                if(UnityEngine.Random.Range(0f, 100f) < Value)
                {
                    samples.RemoveAt(i);
                }
                else
                {
                    onSpawn?.Invoke(samples[i]);
                }
            }
        }
    }
}