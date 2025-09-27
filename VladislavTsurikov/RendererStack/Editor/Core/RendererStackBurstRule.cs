#if UNITY_EDITOR
using VladislavTsurikov.AutoDefines.Editor;

namespace VladislavTsurikov.RendererStack.Editor
{
    public sealed class RendererStackBurstRule : PackagePresenceRule
    {
        public override string GetDefineSymbol() { return "RENDERER_STACK_BURST"; }
        protected override string GetPackageId() { return "com.unity.burst"; }
        protected override bool ShouldAutoInstall() { return false; }
    }
}
#endif
