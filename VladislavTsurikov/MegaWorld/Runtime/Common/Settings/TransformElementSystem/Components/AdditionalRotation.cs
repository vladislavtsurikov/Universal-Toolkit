using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using Transform = VladislavTsurikov.Core.Runtime.Transform;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem.Components
{
    [Serializable]
    [MenuItem("Additional Rotation")]
    public class AdditionalRotation : TransformComponent
    {
        public Vector3 Rotation;

        public override void SetInstanceData(ref Transform transform, float fitness, Vector3 normal)
        {
            transform.Rotation *= Quaternion.Euler(Rotation);
        }
    }
}