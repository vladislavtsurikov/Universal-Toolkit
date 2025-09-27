#if UNITY_EDITOR
using VladislavTsurikov.AutoDefines.Editor;

namespace VladislavTsurikov.RendererStack.Editor
{
    public sealed class RendererStackMathematicsEnsureRule : PackagePresenceRule
    {
        public override string GetDefineSymbol() { return "RENDERER_STACK_MATHEMATICS"; }
        protected override string GetPackageId() { return "com.unity.mathematics"; }
        protected override bool ShouldAutoInstall() { return true; }
    }
}
#endif
