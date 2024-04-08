using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem.Attributes;
using Transform = VladislavTsurikov.Core.Runtime.Transform;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem.Components
{
    [Serializable]
    [Simple]  
    [MenuItem("Align")]
    public class Align : TransformComponent
    {
        public bool UseNormalWeight = true;
        public bool MinMaxRange = true;
		public float MinWeightToNormal;
		public float MaxWeightToNormal = 0.2f;

        public override void SetInstanceData(ref Transform transform, float fitness, Vector3 normal)
        {
            Quaternion normalRotation = Quaternion.FromToRotation(Vector3.up, normal);

            if(UseNormalWeight)
            {
                float normalWeight;

                if(MinMaxRange)
                {
                    normalWeight = UnityEngine.Random.Range(MinWeightToNormal, MaxWeightToNormal);
                }
                else
                {
                    normalWeight = MaxWeightToNormal;
                }

                transform.Rotation *= Quaternion.Lerp(transform.Rotation, normalRotation, normalWeight);
            }
            else
            {
                transform.Rotation *= normalRotation;
            }
        }
    }
}

