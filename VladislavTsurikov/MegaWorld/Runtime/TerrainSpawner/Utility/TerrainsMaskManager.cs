using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem;
using Group = VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Group;

namespace VladislavTsurikov.MegaWorld.Runtime.TerrainSpawner.Utility
{
    public static class TerrainsMaskManager
    {
        private static readonly List<TerrainsMaskItem> _itemList = new List<TerrainsMaskItem>();

        public static float GetFitness(Group group, MaskFilterComponentSettings maskFilterComponentSettings, Vector3 point)
        {
            foreach (var item in _itemList)
            {
                if (item.MaskFilterComponentSettings == maskFilterComponentSettings)
                {
                    return item.GetFitness(group, point);
                }
            }
            
            TerrainsMaskItem localTerrainsMaskItem = new TerrainsMaskItem(maskFilterComponentSettings);
            _itemList.Add(localTerrainsMaskItem);
            return localTerrainsMaskItem.GetFitness(group, point);
        }
        
        public static float GetFitness(Group group, MaskFilterComponentSettings maskFilterComponentSettings, Terrain terrain, Vector3 point)
        {
            foreach (var item in _itemList)
            {
                if (item.MaskFilterComponentSettings == maskFilterComponentSettings)
                {
                    return item.GetFitness(group, terrain, point);
                }
            }
            
            TerrainsMaskItem localTerrainsMaskItem = new TerrainsMaskItem(maskFilterComponentSettings);
            _itemList.Add(localTerrainsMaskItem);
            return localTerrainsMaskItem.GetFitness(group, terrain, point);
        }
        
        public static void Dispose()
        {
            _itemList.Clear();
        }
    }
}