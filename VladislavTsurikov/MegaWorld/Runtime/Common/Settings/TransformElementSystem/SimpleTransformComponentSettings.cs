using VladislavTsurikov.ComponentStack.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem.Components;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem
{
    [MenuItem("Simple Transform Settings")]
    public class SimpleTransformComponentSettings : Component
    {
        public TransformComponentStack Stack = new TransformComponentStack();

        protected override void OnCreate()
        {
            Stack.CreateIfMissingType(typeof(PositionOffset));
            Stack.CreateIfMissingType(typeof(Rotation));
            Stack.CreateIfMissingType(typeof(Scale));
        }
    }
}