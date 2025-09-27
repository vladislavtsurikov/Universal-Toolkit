
namespace VladislavTsurikov.RendererStack.Editor.Core
{
    public sealed class RendererStackMathematicsEnsureRule : UpmPackagePresenceRule
    {
        public override string GetDefineSymbol() { return "RENDERER_STACK_MATHEMATICS"; }
        public override string GetPackageId() { return "com.unity.mathematics"; }
        public override bool ShouldAutoInstall() { return true; }
    }
}
