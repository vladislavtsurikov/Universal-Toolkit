using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem.Components
{
    [Serializable]
    [MenuItem("Additional Rotation")]
    public class AdditionalRotation : TransformComponent
    {
        public Vector3 Rotation;

        public override void SetInstanceData(ref InstanceData instanceData, float fitness, Vector3 normal)
        {
            instanceData.Rotation *= Quaternion.Euler(Rotation);
        }
    }
}