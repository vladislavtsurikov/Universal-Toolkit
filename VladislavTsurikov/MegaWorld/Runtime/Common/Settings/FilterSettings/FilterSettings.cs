using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem;
using VladislavTsurikov.OdinSerializer.Core.Misc;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings
{
    public enum FilterType
    {
        SimpleFilter,
        MaskFilter
    }
    
    [Name("Filter Settings")]
    public class FilterSettings : Component
    {
        public FilterType FilterType = FilterType.MaskFilter;
        
        [OdinSerialize] 
        public SimpleFilter SimpleFilter = new SimpleFilter();
        [OdinSerialize]
        public MaskFilterComponentSettings MaskFilterComponentSettings = new MaskFilterComponentSettings();
    }
}