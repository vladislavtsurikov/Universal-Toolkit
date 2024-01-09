using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.PreferencesSystem;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem
{
    public enum AlphaVisualisationType
    {
        Default,
        BrushFilter,
        None
    }
    
    public enum ColorSpaceForBrushMaskFilter
    {
        СustomColor,
        Colorful,
        Heightmap
    }

    [MenuItem("Visualisation Mask Filters")]
    public class VisualisationMaskFiltersPreference : PreferenceSettings
    {
        public Color Color = new Color(128, 171, 78, 255);
        public bool EnableStripe = true;
        public ColorSpaceForBrushMaskFilter ColorSpace = ColorSpaceForBrushMaskFilter.Colorful;
        public AlphaVisualisationType AlphaVisualisationType = AlphaVisualisationType.None;
        public float CustomAlpha = 0.3f;
    }
}

