using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using Transform = VladislavTsurikov.Core.Runtime.Transform;

namespace VladislavTsurikov.MegaWorld.Editor.BrushModifyTool.ModifyTransformComponents.Elements
{
    [MenuItem("Position")]
    public class Position : ModifyTransformComponent
    {
        public float Strength = -0.1f;
        public float YStrengthRandomize = 100;   

        public override void ModifyTransform(ref Transform spawnInfo, ref ModifyInfo modifyInfo, float moveLenght, Vector3 strokeDirection, float fitness, Vector3 normal)
        {
            float randomPositionY = modifyInfo.RandomPositionY * (YStrengthRandomize / 100f);

            float localStrengthPosition = fitness * moveLenght;

            float addPositionY = Strength * localStrengthPosition * 0.05f;
            addPositionY = Mathf.Lerp(addPositionY, 0, randomPositionY);
    
            Vector3 position = new Vector3(spawnInfo.Position.x, spawnInfo.Position.y + addPositionY, spawnInfo.Position.z);

            spawnInfo.Position = position;
        }
    }
}
