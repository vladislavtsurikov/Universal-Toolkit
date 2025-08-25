using System;
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem
{
    [Serializable]
    [Name("Snap Rotation")]  
    public class SnapRotation : TransformComponent
    {
        [Range(0.1f, 360f)]
        public float SnapRotationAngle = 90f;
        public bool RotateAxisX;
        public bool RotateAxisY = true;
        public bool RotateAxisZ;

        public override void SetInstanceData(ref Instance instance, float fitness, Vector3 normal)
        {
            List<float> rotationValueList = new List<float>();
            int count = 0;
            for (float value = 0; value <= 360f; value += SnapRotationAngle)
            {
                rotationValueList.Add(value);
                count++;
            }

            if(count != 0)
            {
                float randomX = 0;
                float randomY = 0;
                float randomZ = 0;

                if(RotateAxisX)
                {
                    randomX = rotationValueList[UnityEngine.Random.Range(0, rotationValueList.Count)];
                }
                if(RotateAxisY)
                {
                    randomY = rotationValueList[UnityEngine.Random.Range(0, rotationValueList.Count)];
                }
                if(RotateAxisZ)
                {
                    randomZ = rotationValueList[UnityEngine.Random.Range(0, rotationValueList.Count)];
                }

                Quaternion rotationX = Quaternion.AngleAxis(randomX, Vector3.right);
                Quaternion rotationY = Quaternion.AngleAxis(randomY, Vector3.up);
                Quaternion rotationZ = Quaternion.AngleAxis(randomZ, Vector3.forward);

                instance.Rotation = rotationX * rotationY * rotationZ;
            }
        }
    }
}
