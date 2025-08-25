﻿using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem.Attributes;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem
{
    [Serializable]
    [Simple]  
    [Name("Position Offset")]
    public class PositionOffset : TransformComponent
    {
        public float MinPositionOffsetY = -0.15f;
        public float MaxPositionOffsetY;

        public override void SetInstanceData(ref Instance instance, float fitness, Vector3 normal)
        {
            instance.Position += new Vector3(0, UnityEngine.Random.Range(MinPositionOffsetY, MaxPositionOffsetY), 0);
        }
    }
}

