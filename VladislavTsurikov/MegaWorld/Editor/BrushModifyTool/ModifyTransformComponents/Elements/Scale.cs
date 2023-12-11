using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.Runtime;

namespace VladislavTsurikov.MegaWorld.Editor.BrushModifyTool.ModifyTransformComponents.Elements
{
    [MenuItem("Scale")]
    public class Scale : ModifyTransformComponent
    {
        public float Strength = 0.3f;
        public float StrengthRandomize = 100;    

        public override void SetInstanceData(ref InstanceData spawnInfo, ref ModifyInfo modifyInfo, float moveLenght, Vector3 strokeDirection, float fitness, Vector3 normal)
        {
            float randomScale = modifyInfo.RandomScale * (StrengthRandomize / 100f);

            float localStrengthScale = fitness * moveLenght;

            float addScale = Strength * localStrengthScale * 0.005f;
            addScale = Mathf.Lerp(addScale, 0, randomScale);
        
            spawnInfo.Scale = new Vector3(spawnInfo.Scale.x + addScale, spawnInfo.Scale.y + addScale, spawnInfo.Scale.z + addScale);
        }
    }
}