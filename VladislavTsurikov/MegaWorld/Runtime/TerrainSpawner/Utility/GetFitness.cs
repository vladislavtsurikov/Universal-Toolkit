using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime.Scene;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;

namespace VladislavTsurikov.MegaWorld.Runtime.TerrainSpawner.Utility
{
    public static class GetFitness
    {
        public static float Get(Group group, RayHit rayHit)
        {
            FilterSettings filterSettings = (FilterSettings)group.GetElement(typeof(FilterSettings));
            
            if(filterSettings.FilterType == FilterType.MaskFilter)
            {
                return TerrainsMaskManager.GetFitness(group, filterSettings.MaskFilterComponentSettings, rayHit.Point);
            }
            else
            {
                return Common.Utility.GetFitness.GetFromSimpleFilter(filterSettings.SimpleFilter, rayHit);
            }
        }
        
        public static float Get(Group group, MaskFilterComponentSettings maskFilterComponentSettings, Terrain terrain, Vector3 point)
        {
            return TerrainsMaskManager.GetFitness(group, maskFilterComponentSettings, terrain, point);
        }
    }
}