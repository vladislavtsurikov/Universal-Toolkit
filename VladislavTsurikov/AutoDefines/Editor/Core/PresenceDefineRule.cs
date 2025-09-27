#if UNITY_EDITOR

namespace VladislavTsurikov.AutoDefines.Editor.Core
{
    public abstract class PresenceDefineRule : DefineRule
    {
        public abstract string GetDefineSymbol();
        protected abstract bool IsInstalled();
        protected abstract bool OnMissing();

        public sealed override void Run()
        {
            var installed = IsInstalled();

            if (!installed)
            {
                installed = OnMissing();
            }

            if (installed)
            {
                PackageDefineRuleUtility.ApplyDefine(GetDefineSymbol(), installed);
            }
        }
    }
}
#endif
