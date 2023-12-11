using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.PreferencesSystem;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem
{
    [MenuItem("Scatter")]
    public class ScatterPreferenceSettings : PreferenceSettings
    {
        public int MaxChecks = 100;
    }
}