using VladislavTsurikov.ComponentStack.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem.Components;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.PhysicsToolsSettings
{
    [MenuItem("Physics Transform Settings")]
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