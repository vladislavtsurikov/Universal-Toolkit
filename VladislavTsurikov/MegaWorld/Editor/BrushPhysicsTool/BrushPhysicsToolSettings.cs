using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.MegaWorld.Runtime.Common.PhysXPainter.Settings;

namespace VladislavTsurikov.MegaWorld.Editor.BrushPhysicsTool
{
    [Name("Brush Physics Tool Settings")]
    public class BrushPhysicsToolSettings : Component
    {
        public PhysicsEffects PhysicsEffects = new PhysicsEffects();
        public float PositionOffsetY = 30;
    }
}