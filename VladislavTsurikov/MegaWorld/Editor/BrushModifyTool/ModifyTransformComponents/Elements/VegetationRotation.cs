using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.Runtime;

namespace VladislavTsurikov.MegaWorld.Editor.BrushModifyTool.ModifyTransformComponents.Elements
{
    [MenuItem("Vegetation Rotation")]
    public class VegetationRotation : ModifyTransformComponent
    {
        public float StrengthY = 7;
        public float StrengthXY = 10;
        public float RotationXZ = 3;

        public override void SetInstanceData(ref InstanceData instanceData, ref ModifyInfo modifyInfo, float moveLenght, Vector3 strokeDirection, float fitness, Vector3 normal)
        {
            float localstrengthRotationY  = StrengthY * fitness;
            float localstrengthRotationXY = StrengthXY * fitness;

            Vector3 randomVector = modifyInfo.RandomRotation * 0.5f;
            float angleXZ = RotationXZ * 3.6f;
            float t = localstrengthRotationXY / 100;

            float rotationY = localstrengthRotationY * 3.6f * randomVector.y + instanceData.Rotation.eulerAngles.y;
            float rotationX = angleXZ;
            float rotationZ = angleXZ;

            instanceData.Rotation = Quaternion.Euler(new Vector3(instanceData.Rotation.eulerAngles.x, rotationY, instanceData.Rotation.eulerAngles.z));  
            
            Quaternion rotation = Quaternion.Euler(new Vector3(rotationX, rotationY, rotationZ));    
            Quaternion finalRotation = Quaternion.Lerp(instanceData.Rotation, rotation, t);          

            instanceData.Rotation = finalRotation;
        }
    }
}