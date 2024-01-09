using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime.Scene;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Stamper;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;

namespace VladislavTsurikov.MegaWorld.Runtime.GravitySpawner.Utility
{
    public static class GetFitness
    {
        public static float Get(Group group, TerrainsMaskManager terrainsMaskManager, RayHit rayHit)
        {
            FilterSettings filterSettings = (FilterSettings)group.GetElement(typeof(GravitySpawner), typeof(FilterSettings));
            
            if(filterSettings.FilterType == FilterType.MaskFilter)
            {
                return terrainsMaskManager.GetFitness(group, filterSettings.MaskFilterComponentSettings, rayHit.Point);
            }
            else
            {
                return Common.Utility.GetFitness.GetFromSimpleFilter(filterSettings.SimpleFilter, rayHit);
            }
        }
        
        public static float Get(Group group, TerrainsMaskManager terrainsMaskManager, MaskFilterComponentSettings maskFilterComponentSettings, Terrain terrain, Vector3 point)
        {
            return terrainsMaskManager.GetFitness(group, maskFilterComponentSettings, terrain, point);
        }
    }
}