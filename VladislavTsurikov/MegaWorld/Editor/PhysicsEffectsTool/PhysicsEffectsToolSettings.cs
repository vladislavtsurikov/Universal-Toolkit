#if UNITY_EDITOR
using System;
using VladislavTsurikov.ComponentStack.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.MegaWorld.Editor.PhysicsEffectsTool.PhysicsEffectsSystem;

namespace VladislavTsurikov.MegaWorld.Editor.PhysicsEffectsTool
{
    [Serializable]
    [Name("Physics Effects Tool Settings")]
    public class PhysicsEffectsToolSettings : Component
    {
        public ComponentStackOnlyDifferentTypes<PhysicsEffect> List = new ComponentStackOnlyDifferentTypes<PhysicsEffect>();
        
        public float Spacing = 5;

        protected override void SetupComponent(object[] setupData = null)
        {
            List.CreateAllElementTypes();
        }
    }
}
#endif