using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.Runtime;

namespace VladislavTsurikov.MegaWorld.Editor.BrushModifyTool.ModifyTransformComponents.Elements
{
    [MenuItem("Along")]
    public class Along : ModifyTransformComponent
    {
        public override void SetInstanceData(ref InstanceData instanceData, ref ModifyInfo modifyInfo, float moveLenght, Vector3 strokeDirection, float fitness, Vector3 normal)
        {
            Quaternion rotation = Quaternion.identity;

            Vector3 upwards = new Vector3(0, 1, 0);

            float strength = Mathf.InverseLerp(0, 15, moveLenght);
            strength *= fitness;

            Vector3 forward = Vector3.Cross(strokeDirection, upwards);
            
            rotation = Quaternion.LookRotation(forward, upwards);

            instanceData.Rotation = Quaternion.Lerp(instanceData.Rotation, rotation, strength);
        } 
    }
}