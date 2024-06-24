﻿#if UNITY_EDITOR
using VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings;
using VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings.MaskFilterSystem;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Common.Utility.Repaint;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;

namespace VladislavTsurikov.MegaWorld.Editor.AdvancedBrushTool
{
    public static class AdvancedBrushToolVisualisation
    {
        private static MaskFilterVisualisation _maskFilterVisualisation = new MaskFilterVisualisation();
        
        public static void Draw(BoxArea area, SelectionData data, LayerSettings layerSettings)
        {
            if(area == null || area.RayHit == null)
            {
                return;
            }
            
            if(data.SelectedData.HasOneSelectedGroup())
            {
                Group group = data.SelectedData.SelectedGroup;

                if (group.PrototypeType == typeof(PrototypeGameObject) || group.PrototypeType == typeof(PrototypeTerrainObject))
                {
                    FilterSettings filterSettings = (FilterSettings)group.GetElement(typeof(FilterSettings));
                                            
                    if(filterSettings.FilterType != FilterType.MaskFilter)
                    {
                        SimpleFilterVisualisation.DrawSimpleFilter(group, area, filterSettings.SimpleFilter, layerSettings);
                        VisualisationUtility.DrawCircleHandles(area.BoxSize, area.RayHit);
                    }
                    else
                    {
                        _maskFilterVisualisation.DrawMaskFilterVisualization(filterSettings.MaskFilterComponentSettings.MaskFilterStack, area);
                    }
                }
                else
                {
                    if(data.SelectedData.HasOneSelectedPrototype())
                    {
                        _maskFilterVisualisation.DrawMaskFilterVisualization(MaskFilterUtility.GetMaskFilterFromSelectedPrototype(data), area);
                    }
                    else
                    {
                        DrawShaderVisualisationUtility.DrawAreaPreview(area);
                    }
                }
            }
            else
            {
                if(SimpleFilterUtility.HasOneActiveSimpleFilter(typeof(AdvancedBrushTool), WindowData.Instance.SelectedData))
                {
                    VisualisationUtility.DrawCircleHandles(area.BoxSize, area.RayHit);
                }
                else
                {
                    DrawShaderVisualisationUtility.DrawAreaPreview(area);
                }
            }
        }
    }
}
#endif