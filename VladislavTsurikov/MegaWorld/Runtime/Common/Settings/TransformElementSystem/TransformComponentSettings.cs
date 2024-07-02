using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Core;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem
{
    [Name("Transform Settings")]
    public class TransformComponentSettings : Component
    {
        public TransformComponentStack TransformComponentStack = new TransformComponentStack();

        protected override void OnCreate()
		{
			TransformComponentStack.CreateIfMissingType(typeof(PositionOffset));
			TransformComponentStack.CreateIfMissingType(typeof(Rotation));
			TransformComponentStack.CreateIfMissingType(typeof(Scale));
		}
    }
}