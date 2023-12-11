using VladislavTsurikov.ComponentStack.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.MegaWorld.Editor.BrushModifyTool.ModifyTransformComponents.Elements;

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
    }
}