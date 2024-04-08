using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem.Attributes;
using Transform = VladislavTsurikov.Core.Runtime.Transform;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem.Components
{
    [Serializable]
    [Simple]  
    [MenuItem("Position Offset")]
    public class PositionOffset : TransformComponent
    {
        public float MinPositionOffsetY = -0.15f;
        public float MaxPositionOffsetY;

        public override void SetInstanceData(ref Transform transform, float fitness, Vector3 normal)
        {
            transform.Position += new Vector3(0, UnityEngine.Random.Range(MinPositionOffsetY, MaxPositionOffsetY), 0);
        }
    }
}

