#if UNITY_EDITOR
using VladislavTsurikov.AutoDefines.Editor;

namespace VladislavTsurikov.SceneManagerTool.Editor
{
    public sealed class SceneManagerAddressablesRule : PackagePresenceRule
    {
        public override string GetDefineSymbol() { return "SCENE_MANAGER_ADDRESSABLES"; }
        protected override string GetPackageId() { return "com.unity.addressables"; }
        protected override bool ShouldAutoInstall() { return false; }
    }
}
#endif
