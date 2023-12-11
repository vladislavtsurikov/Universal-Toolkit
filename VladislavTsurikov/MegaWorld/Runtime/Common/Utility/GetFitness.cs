using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime.Scene;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.Utility.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Utility
{
    public static class GetFitness
    {
        public static float Get(Group group, Bounds bounds, RayHit rayHit)
        {
            FilterSettings filterSettings = (FilterSettings)group.GetElement(typeof(FilterSettings));
            
            if(filterSettings.FilterType == FilterType.MaskFilter)
            {
                return GetFromMaskFilter(bounds, filterSettings.MaskFilterComponentSettings.MaskFilterStack, filterSettings.MaskFilterComponentSettings.FilterMaskTexture2D, rayHit.Point);
            }
            else
            {
                return GetFromSimpleFilter(filterSettings.SimpleFilter, rayHit);
            }
        }

        public static float GetFromSimpleFilter(SimpleFilter simpleFilter, RayHit rayHit)
        {
            return simpleFilter.GetFitness(rayHit.Point, rayHit.Normal);
        }

        public static float GetFromMaskFilter(Bounds bounds, MaskFilterStack stack, Texture2D filterMask, Vector3 point)
        {
            if(stack.ElementList.Count != 0)
            {
                return GrayscaleFromTexture.GetFromWorldPosition(bounds, point, filterMask);
            }

            return 1;
        }
    }
}