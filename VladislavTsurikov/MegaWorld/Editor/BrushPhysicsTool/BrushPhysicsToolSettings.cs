#if UNITY_EDITOR
using System;
using VladislavTsurikov.ComponentStack.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.PhysicsToolsSettings;

namespace VladislavTsurikov.MegaWorld.Editor.BrushPhysicsTool
{
    [Serializable]
    [MenuItem("Brush Physics Tool Settings")]
    public class BrushPhysicsToolSettings : Component
    {
        public PhysicsEffects PhysicsEffects = new PhysicsEffects();
        public float PositionOffsetY = 30;
    }
}
#endif