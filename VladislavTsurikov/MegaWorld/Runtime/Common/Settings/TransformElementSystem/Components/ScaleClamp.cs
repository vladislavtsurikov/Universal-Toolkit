using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using Transform = VladislavTsurikov.Runtime.Transform;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem.Components
{
    [Serializable]
    [MenuItem("Scale Clamp")]
    public class ScaleClamp : TransformComponent
    {
        public float MaxScale = 2;
        public float MinScale = 0.5f;

        public override void SetInstanceData(ref Transform transform, float fitness, Vector3 normal)
        {
            if(transform.Scale.x > MaxScale)
            {
                transform.Scale.x = MaxScale;
            }
            else if(transform.Scale.x < MinScale)
            {
                transform.Scale.x = MinScale;
            }

            if(transform.Scale.y > MaxScale)
            {
                transform.Scale.y = MaxScale;
            }
            else if(transform.Scale.y < MinScale)
            {
                transform.Scale.y = MinScale;
            }

            if(transform.Scale.z > MaxScale)
            {
                transform.Scale.z = MaxScale;
            }
            else if(transform.Scale.z < MinScale)
            {
                transform.Scale.z = MinScale;
            }
        }
    }
}