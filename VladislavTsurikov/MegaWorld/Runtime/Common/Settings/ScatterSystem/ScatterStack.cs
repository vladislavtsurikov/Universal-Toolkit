using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem
{
    [Serializable]
    public class ScatterStack : ComponentStackOnlyDifferentTypes<Scatter>
    {
        public float SecondsUntilNextFrame = 5;
        public float MillisecondsUntilNextFrame => SecondsUntilNextFrame * 1000;
        public bool WaitForNextFrame;

        public IEnumerator Samples(BoxArea boxArea, Action<Vector2> onAddSample)
        {
            List<Scatter> enabledScatter = new List<Scatter>(_elementList.FindAll(scatter => scatter.Active));

            List<Vector2> samples = new List<Vector2>();
            
            for (int i = 0; i < enabledScatter.Count; i++)
            {
                yield return enabledScatter[i].Samples(boxArea, samples, i == enabledScatter.Count - 1 ? onAddSample : null);
            }
        }
    }
}