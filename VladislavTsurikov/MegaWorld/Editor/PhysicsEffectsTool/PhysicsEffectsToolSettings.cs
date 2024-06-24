#if UNITY_EDITOR
using System;
using VladislavTsurikov.ComponentStack.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.MegaWorld.Editor.PhysicsEffectsTool.PhysicsEffectsSystem;

namespace VladislavTsurikov.MegaWorld.Editor.PhysicsEffectsTool
{
    [Serializable]
    [MenuItem("Physics Effects Tool Settings")]
    public class PhysicsEffectsToolSettings : Component
    {
        public ComponentStackOnlyDifferentTypes<PhysicsEffect> List = new ComponentStackOnlyDifferentTypes<PhysicsEffect>();
        
        public float Spacing = 5;

        protected override void SetupElement(object[] args = null)
        {
            List.CreateAllElementTypes();
        }
    }
}
#endif