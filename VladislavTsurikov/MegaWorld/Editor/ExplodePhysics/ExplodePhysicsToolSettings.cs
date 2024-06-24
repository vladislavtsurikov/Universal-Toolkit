#if UNITY_EDITOR
using VladislavTsurikov.ComponentStack.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Core;

namespace VladislavTsurikov.MegaWorld.Editor.ExplodePhysics
{
    [MenuItem("Explode Physics Tool Settings")]
    public class ExplodePhysicsToolSettings : Component
    {
        public float Spacing = 20;
        public float PositionOffsetY = 15;
        public float Size = 10;
        public int InstancesMin = 25;
        public int InstancesMax = 50;
        public bool SpawnFromOnePoint;
        public float Force = 20f;
    }
}
#endif