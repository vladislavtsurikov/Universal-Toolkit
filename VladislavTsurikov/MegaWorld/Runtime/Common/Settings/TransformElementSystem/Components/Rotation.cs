using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem.Attributes;
using Transform = VladislavTsurikov.Core.Runtime.Transform;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem.Components
{
    [Serializable]
    [Simple]  
    [MenuItem("Rotation")]
    public class Rotation : TransformComponent
    {
        public float RandomizeOrientationX = 5;
        public float RandomizeOrientationY = 100;
        public float RandomizeOrientationZ = 5;

        public override void SetInstanceData(ref Transform transform, float fitness, Vector3 normal)
        {
            Vector3 randomVector = UnityEngine.Random.insideUnitSphere * 0.5f;
            Quaternion randomRotation = Quaternion.Euler(new Vector3(
                RandomizeOrientationX * 3.6f * randomVector.x,
                RandomizeOrientationY * 3.6f * randomVector.y,
                RandomizeOrientationZ * 3.6f * randomVector.z));

            transform.Rotation = randomRotation;
        }
    }
}
