using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Editor.BrushModifyTool.ModifyTransformComponents
{
    [MenuItem("Vegetation Rotation")]
    public class VegetationRotation : ModifyTransformComponent
    {
        public float StrengthY = 7;
        public float StrengthXY = 10;
        public float RotationXZ = 3;

        public override void ModifyTransform(ref Instance instance, ref ModifyInfo modifyInfo, float moveLenght, Vector3 strokeDirection, float fitness, Vector3 normal)
        {
            float localstrengthRotationY  = StrengthY * fitness;
            float localstrengthRotationXY = StrengthXY * fitness;

            Vector3 randomVector = modifyInfo.RandomRotation * 0.5f;
            float angleXZ = RotationXZ * 3.6f;
            float t = localstrengthRotationXY / 100;

            float rotationY = localstrengthRotationY * 3.6f * randomVector.y + instance.Rotation.eulerAngles.y;
            float rotationX = angleXZ;
            float rotationZ = angleXZ;

            instance.Rotation = Quaternion.Euler(new Vector3(instance.Rotation.eulerAngles.x, rotationY, instance.Rotation.eulerAngles.z));  
            
            Quaternion rotation = Quaternion.Euler(new Vector3(rotationX, rotationY, rotationZ));    
            Quaternion finalRotation = Quaternion.Lerp(instance.Rotation, rotation, t);          

            instance.Rotation = finalRotation;
        }
    }
}