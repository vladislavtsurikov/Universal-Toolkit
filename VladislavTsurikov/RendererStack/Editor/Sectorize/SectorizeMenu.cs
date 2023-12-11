#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.RendererStack.Editor.Core.RendererSystem;
using VladislavTsurikov.RendererStack.Runtime.Core.RendererSystem;
using GUIUtility = VladislavTsurikov.Utility.Runtime.GUIUtility;

namespace VladislavTsurikov.RendererStack.Editor.Sectorize
{
    public class SectorizeMenu : RendererMenu
    {
        public override void ShowGenericMenu(GenericMenu menu, CustomRenderer customRenderer)
        {
            Runtime.Sectorize.Sectorize renderer = (Runtime.Sectorize.Sectorize)customRenderer;

            menu.AddItem(new GUIContent("Debug/All Cells"), renderer.DebugAllCells, GUIUtility.ContextMenuCallback, new Action(() => 
                renderer.DebugAllCells = !renderer.DebugAllCells));
            
            menu.AddItem(new GUIContent("Debug/Visible Cells"), renderer.DebugVisibleCells, GUIUtility.ContextMenuCallback, new Action(() => 
                renderer.DebugVisibleCells = !renderer.DebugVisibleCells));
        }
    }
}
#endif