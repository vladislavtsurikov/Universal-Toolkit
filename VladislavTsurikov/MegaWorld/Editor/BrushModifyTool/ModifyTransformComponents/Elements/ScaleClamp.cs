using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.Runtime;

namespace VladislavTsurikov.MegaWorld.Editor.BrushModifyTool.ModifyTransformComponents.Elements
{
    [MenuItem("Scale Clamp")]
    public class ScaleClamp : ModifyTransformComponent
    {
        public float MaxScale = 2;
        public float MinScale = 0.5f;

        public override void SetInstanceData(ref InstanceData instanceData, ref ModifyInfo modifyInfo, float moveLenght, Vector3 strokeDirection, float fitness, Vector3 normal)
        {
            Vector3 scale = instanceData.Scale;

            if(instanceData.Scale.x > MaxScale)
            {
                scale.x = MaxScale;
            }
            else if(instanceData.Scale.x < MinScale)
            {
                scale.x = MinScale;
            }

            if(instanceData.Scale.y > MaxScale)
            {
                scale.y = MaxScale;
            }
            else if(instanceData.Scale.y < MinScale)
            {
                scale.y = MinScale;
            }

            if(instanceData.Scale.z > MaxScale)
            {
                scale.z = MaxScale;
            }
            else if(instanceData.Scale.z < MinScale)
            {
                scale.z = MinScale;
            }

            instanceData.Scale = Vector3.Lerp(instanceData.Scale, scale, fitness);
        }
    }
}