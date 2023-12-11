using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem.Components
{
    [Serializable]
    [MenuItem("Cliffs Position")]  
    public class CliffsPosition : TransformComponent
    {
        public float OffsetPosition = 1;

        public override void SetInstanceData(ref InstanceData instanceData, float fitness, Vector3 normal)
        {
            Vector3 direction = new Vector3(normal.x, 0, normal.z);

            instanceData.Position += direction + new Vector3(OffsetPosition, 0, OffsetPosition);
        }
    }
}