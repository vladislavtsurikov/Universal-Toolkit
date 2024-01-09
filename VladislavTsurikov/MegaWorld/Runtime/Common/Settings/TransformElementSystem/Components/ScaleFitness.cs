using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using Transform = VladislavTsurikov.Runtime.Transform;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem.Components
{
    [Serializable]
    [MenuItem("Scale Fitness")]  
    public class ScaleFitness : TransformComponent
    {
        public float OffsetScale = -0.7f;

        public override void SetInstanceData(ref Transform transform, float fitness, Vector3 normal)
        {
            float value = Mathf.Lerp(OffsetScale, 0, fitness);

            transform.Scale += new Vector3(value, value, value);
        }
    }
}