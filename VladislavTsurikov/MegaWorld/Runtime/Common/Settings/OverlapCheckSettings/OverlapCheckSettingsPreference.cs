using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.PreferencesSystem;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.OverlapCheckSettings
{
    [MenuItem("Overlap Check Settings")]
    public class OverlapCheckSettingsPreference : PreferenceSettings
    {
        public float MultiplyFindSize = 1f;
    }
}
