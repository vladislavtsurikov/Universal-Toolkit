using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using Transform = VladislavTsurikov.Runtime.Transform;

namespace VladislavTsurikov.MegaWorld.Editor.BrushModifyTool.ModifyTransformComponents.Elements
{
    [MenuItem("Scale Clamp")]
    public class ScaleClamp : ModifyTransformComponent
    {
        public float MaxScale = 2;
        public float MinScale = 0.5f;

        public override void SetInstanceData(ref Transform transform, ref ModifyInfo modifyInfo, float moveLenght, Vector3 strokeDirection, float fitness, Vector3 normal)
        {
            Vector3 scale = transform.Scale;

            if(transform.Scale.x > MaxScale)
            {
                scale.x = MaxScale;
            }
            else if(transform.Scale.x < MinScale)
            {
                scale.x = MinScale;
            }

            if(transform.Scale.y > MaxScale)
            {
                scale.y = MaxScale;
            }
            else if(transform.Scale.y < MinScale)
            {
                scale.y = MinScale;
            }

            if(transform.Scale.z > MaxScale)
            {
                scale.z = MaxScale;
            }
            else if(transform.Scale.z < MinScale)
            {
                scale.z = MinScale;
            }

            transform.Scale = Vector3.Lerp(transform.Scale, scale, fitness);
        }
    }
}