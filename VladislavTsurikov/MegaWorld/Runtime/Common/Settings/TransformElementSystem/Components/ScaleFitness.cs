﻿using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem.Components
{
    [Serializable]
    [MenuItem("Scale Fitness")]  
    public class ScaleFitness : TransformComponent
    {
        public float OffsetScale = -0.7f;

        public override void SetInstanceData(ref InstanceData instanceData, float fitness, Vector3 normal)
        {
            float value = Mathf.Lerp(OffsetScale, 0, fitness);

            instanceData.Scale += new Vector3(value, value, value);
        }
    }
}