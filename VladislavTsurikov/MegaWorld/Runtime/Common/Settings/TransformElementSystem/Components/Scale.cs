using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem.Attributes;
using VladislavTsurikov.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem.Components
{
    [Serializable]
    [Simple]  
    [MenuItem("Scale")]
    public class Scale : TransformComponent
    {
        public bool UniformScale = true;
        public Vector3 MinScale = new Vector3(0.8f, 0.8f, 0.8f);
        public Vector3 MaxScale = new Vector3(1.2f, 1.2f, 1.2f);

        public override void SetInstanceData(ref InstanceData instanceData, float fitness, Vector3 normal)
        {
            if (UniformScale)
            {
                float resScale = UnityEngine.Random.Range(MinScale.x, MaxScale.x);
                instanceData.Scale = new Vector3(resScale, resScale, resScale);
            }
            else
            {
                instanceData.Scale = new Vector3(
                    UnityEngine.Random.Range(MinScale.x, MaxScale.x),
                    UnityEngine.Random.Range(MinScale.y, MaxScale.y),
                    UnityEngine.Random.Range(MinScale.z, MaxScale.z));
            }
        }
    }
}

