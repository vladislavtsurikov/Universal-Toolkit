using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem.Attributes;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem
{
    [Serializable]
    [Simple]  
    [Name("Align")]
    public class Align : TransformComponent
    {
        public bool UseNormalWeight = true;
        public bool MinMaxRange = true;
		public float MinWeightToNormal;
		public float MaxWeightToNormal = 0.2f;

        public override void SetInstanceData(ref Instance instance, float fitness, Vector3 normal)
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

                instance.Rotation *= Quaternion.Lerp(instance.Rotation, normalRotation, normalWeight);
            }
            else
            {
                instance.Rotation *= normalRotation;
            }
        }
    }
}

