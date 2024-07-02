using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.PhysXPainter.Settings
{
    [Name("Physics Transform Settings")]
    public class PhysicsTransformComponentSettings : Component
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