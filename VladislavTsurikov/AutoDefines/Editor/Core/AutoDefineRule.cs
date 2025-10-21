#if UNITY_EDITOR

namespace VladislavTsurikov.AutoDefines.Editor.Core
{
    public abstract class AutoDefineRule
    {
        protected abstract bool IsInstalled();
        protected abstract string GetDefineToApplySymbol();

        protected virtual void InstallIfMissing()
        {
        }

        public void Run()
        {
            if (ApplyDefineIsInstalled())
            {
                return;
            }

            InstallIfMissing();
        }

        private bool ApplyDefineIsInstalled()
        {
            if (IsInstalled())
            {
                DefineSymbolsBatcher.ApplyDefine(GetDefineToApplySymbol(), true);
                return true;
            }

            return false;
        }
    }
}
#endif
