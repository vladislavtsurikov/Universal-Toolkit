#if UNITY_EDITOR
using System;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.MegaWorld.Editor.PhysicsEffectsTool.PhysicsEffectsSystem;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Editor.PhysicsEffectsTool
{
    [Serializable]
    [Name("Physics Effects Tool Settings")]
    public class PhysicsEffectsToolSettings : Component
    {
        public float Spacing = 5;
        public ComponentStackOnlyDifferentTypes<PhysicsEffect> List = new();

        protected override UniTask SetupComponent(object[] setupData = null)
        {
            List.CreateAllElementTypes();
            return UniTask.CompletedTask;
        }
    }
}
#endif
