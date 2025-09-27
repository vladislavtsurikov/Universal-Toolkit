#if UNITY_EDITOR
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using VladislavTsurikov.AutoDefines.Editor.Core;

namespace VladislavTsurikov.AutoDefines.Editor
{
    public abstract class PackagePresenceRule : PresenceDefineRule
    {
        private AddRequest _addRequest;

        protected abstract string GetPackageId();

        protected virtual bool ShouldAutoInstall()
        {
            return false;
        }

        protected override bool IsInstalled()
        {
            return UpmListCache.IsInstalled(GetPackageId());;
        }

        protected override bool OnMissing()
        {
            if (!ShouldAutoInstall())
            {
                return false;
            }

            UpmInstaller.Add(GetPackageId());

            if (UpmInstaller.IsInstalling)
            {

            }

            _addRequest = Client.Add(GetDefineSymbol());

            if (_addRequest == null)
            {
                return false;
            }

            if (_addRequest.IsCompleted)
            {
                _addRequest = null;
            }

            return true;
        }
    }
}
#endif
