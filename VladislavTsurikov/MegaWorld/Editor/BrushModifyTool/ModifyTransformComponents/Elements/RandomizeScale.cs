using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Editor.BrushModifyTool.ModifyTransformComponents
{
    [Name("Randomize Scale")]
    public class RandomizeScale : ModifyTransformComponent
    {
        public bool UniformScale = true;
        public Vector3 MinScale = new Vector3(0.8f, 0.8f, 0.8f);
        public Vector3 MaxScale = new Vector3(1.2f, 1.2f, 1.2f);

        public override void ModifyTransform(ref Instance instance, ref ModifyInfo modifyInfo, float moveLenght, Vector3 strokeDirection, float fitness, Vector3 normal)
        {
            Vector3 scale;
            if (UniformScale)
            {
                float resScale = Random.Range(MinScale.x, MaxScale.x); 
                scale = new Vector3(resScale, resScale, resScale);
            }
            else
            {
                scale = new Vector3(
                    Random.Range(MinScale.x, MaxScale.x),
                    Random.Range(MinScale.y, MaxScale.y),
                    Random.Range(MinScale.z, MaxScale.z));
            }

            instance.Scale = Vector3.Lerp(instance.Scale, scale, fitness);
        }
    }
}