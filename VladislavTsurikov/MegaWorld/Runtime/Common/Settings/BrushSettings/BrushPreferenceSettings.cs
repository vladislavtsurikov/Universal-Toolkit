using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.PreferencesSystem;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.BrushSettings
{
    [MenuItem("Brush")]
    public class BrushPreferenceSettings : PreferenceSettings
    {
        public float MaxBrushSize = 200;
    }
}