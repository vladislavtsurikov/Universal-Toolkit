using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using Transform = VladislavTsurikov.Core.Runtime.Transform;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem.Components
{
    [Serializable]
    [MenuItem("Cliffs Align")]
    public class CliffsAlign : TransformComponent
    {
        public override void SetInstanceData(ref Transform transform, float fitness, Vector3 normal)
        {
            Vector3 direction = new Vector3(normal.x, 0, normal.z);

            float distancePositive = Vector3.Distance(Vector3.right, direction);
            float distanceNegative = Vector3.Distance(-Vector3.right, direction);

            if(distancePositive < distanceNegative)
            {
                float angle = Vector3.Angle(Vector3.forward, direction);
                transform.Rotation = Quaternion.AngleAxis(angle, Vector3.up) * transform.Rotation;
            }
            else
            {
                float angle = -Vector3.Angle(Vector3.forward, direction);
                transform.Rotation = Quaternion.AngleAxis(angle, Vector3.up) * transform.Rotation;
            }
        }
    }
}