﻿using System;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas;
using Group = VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Group;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.Utility
{
    public class SimpleFilterUtility
    {
        public static bool HasOneActiveSimpleFilter(Type toolType, SelectedData selectedVariables)
        {
            foreach (Group group in selectedVariables.SelectedGroupList)
            {
                FilterSettings filterComponent = (FilterSettings)group.GetElement(toolType, typeof(FilterSettings));

                if (filterComponent == null) 
                {
                    continue;
                }
                
                if(filterComponent.FilterType == FilterType.SimpleFilter)
                {
                    return true;
                }
            }

            return false;
        }
    }
}