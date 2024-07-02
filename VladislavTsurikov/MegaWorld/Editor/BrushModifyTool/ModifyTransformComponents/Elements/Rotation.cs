using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Editor.BrushModifyTool.ModifyTransformComponents
{
    [Name("Rotation")]
    public class Rotation : ModifyTransformComponent
    {
        public float ModifyStrengthRotation = 3;
        public bool ModifyRandomRotationX = true;
        public bool ModifyRandomRotationY = true;
        public bool ModifyRandomRotationZ = true;
        public Vector3 ModifyRandomRotationValues = new Vector3(1, 1, 1);
        public Vector3 ModifyRotationValues = new Vector3(1, 1, 1);

        public override void ModifyTransform(ref Instance spawnInfo, ref ModifyInfo modifyInfo, float moveLenght, Vector3 strokeDirection, float fitness, Vector3 normal)
        {
            Vector3 modifyRotation = GetModifyRotation(modifyInfo);

            float localStrengthRotation = ModifyStrengthRotation * fitness * moveLenght;
        
            Quaternion rotation = spawnInfo.Rotation * Quaternion.Euler(modifyRotation * localStrengthRotation * 0.1f);
        
            spawnInfo.Rotation = rotation;
        }

        public Vector3 GetModifyRotation(ModifyInfo modifyInfo)
        {
            float x = 0;
            float y = 0;
            float z = 0;

            if(ModifyRandomRotationX)
            {
                x = ModifyRandomRotationValues.x * modifyInfo.RandomRotation.x;
            }
            else
            {
                x = ModifyRotationValues.x;
            }

            if(ModifyRandomRotationY)
            {
                y = ModifyRandomRotationValues.y * modifyInfo.RandomRotation.y;
            }
            else
            {
                y = ModifyRotationValues.y;
            }

            if(ModifyRandomRotationZ)
            {
                z = ModifyRandomRotationValues.z * modifyInfo.RandomRotation.z;
            }
            else
            {
                z = ModifyRotationValues.z;
            }
        
            return new Vector3(x, y, z);
        }
    }
}