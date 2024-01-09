﻿using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using Transform = VladislavTsurikov.Runtime.Transform;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem.Components
{
    [Serializable]
    [MenuItem("Tree Rotation")]  
    public class TreeRotation : TransformComponent
    {
        public float RandomizeOrientationY = 100;
        public float RandomizeOrientationXZ = 3;
        public float SuccessRandomizeOrientationXZ = 20;

        public override void SetInstanceData(ref Transform transform, float fitness, Vector3 normal)
        {
            Vector3 randomVector = UnityEngine.Random.insideUnitSphere * 0.5f;

            float randomSuccess = UnityEngine.Random.Range(0.0f, 1.0f);

            if(randomSuccess < SuccessRandomizeOrientationXZ / 100)
            {
                Quaternion randomRotation = Quaternion.Euler(new Vector3(
                RandomizeOrientationXZ * 3.6f * randomVector.x,
                RandomizeOrientationY * 3.6f * randomVector.y,
                RandomizeOrientationXZ * 3.6f * randomVector.z));

                transform.Rotation *= randomRotation;
            }
            else
            {
                Quaternion randomRotation = Quaternion.Euler(new Vector3(0, RandomizeOrientationY * 3.6f * randomVector.y, 0));

                transform.Rotation *= randomRotation;
            }
        }
    }
}