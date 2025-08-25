﻿using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem
{
    [Serializable]
    [Name("Slope Position")]  
    public class SlopePosition : TransformComponent
    {
        public float MaxSlope = 90;
        public float PositionOffsetY = -1;

        public override void SetInstanceData(ref Instance instance, float fitness, Vector3 normal)
        {
            if (normal == Vector3.zero)
            {
                return;
            }

            float normalAngle = Vector3.Angle(normal, Vector3.up);
            float difference = normalAngle / MaxSlope;
            
            float positionY = Mathf.Lerp(0, PositionOffsetY, difference);
            
            instance.Position += new Vector3(0, positionY, 0);
        }
    }
}
