using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Editor.BrushModifyTool.ModifyTransformComponents
{
    [Name("Along")]
    public class Along : ModifyTransformComponent
    {
        public override void ModifyTransform(ref Instance instance, ref ModifyInfo modifyInfo, float moveLenght, Vector3 strokeDirection, float fitness, Vector3 normal)
        {
            Vector3 upwards = new Vector3(0, 1, 0);

            float strength = Mathf.InverseLerp(0, 15, moveLenght);
            strength *= fitness;

            Vector3 forward = Vector3.Cross(strokeDirection, upwards);
            
            var rotation = Quaternion.LookRotation(forward, upwards);

            instance.Rotation = Quaternion.Lerp(instance.Rotation, rotation, strength);
        } 
    }
}