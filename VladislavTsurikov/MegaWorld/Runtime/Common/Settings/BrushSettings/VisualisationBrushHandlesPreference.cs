using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.PreferencesSystem;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.BrushSettings
{
    [MenuItem("Visualisation Brush Handles")]
    public class VisualisationBrushHandlesPreference : PreferenceSettings
    {
        public bool DrawSolidDisc = true;
        public Color CircleColor = new Color(0.2f, 0.5f, 0.7f, 1);
        public float CirclePixelWidth = 5f;
    }
}

