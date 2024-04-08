using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.MegaWorld.Editor.BrushModifyTool.ModifyTransformComponents.Elements;
using Component = VladislavTsurikov.ComponentStack.Runtime.Component;
using Transform = VladislavTsurikov.Core.Runtime.Transform;

namespace VladislavTsurikov.MegaWorld.Editor.BrushModifyTool.ModifyTransformComponents
{
    [MenuItem("Modify Transform Components")]
    public class ModifyTransformSettings : Component
    {
        public ComponentStackOnlyDifferentTypes<ModifyTransformComponent> Stack = new ComponentStackOnlyDifferentTypes<ModifyTransformComponent>();

        protected override void OnCreate()
        {
            Stack.CreateIfMissingType(typeof(RandomizeScale));
        }
        
        public void ModifyTransform(ref Transform transform, ref ModifyInfo modifyInfo, float moveLenght, Vector3 strokeDirection, float fitness, Vector3 normal)
        {
            foreach (ModifyTransformComponent item in Stack.ElementList)
            {
                if(item.Active)
                {
                    item.ModifyTransform(ref transform, ref modifyInfo, moveLenght, strokeDirection, fitness, normal);
                }
            }
        }
    }
}