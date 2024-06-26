﻿#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.RendererStack.Editor.Core.RendererSystem;
using VladislavTsurikov.RendererStack.Runtime.Core.RendererSystem;
using VladislavTsurikov.UnityUtility.Editor;
using VladislavTsurikov.Utility.Runtime;
using Renderer = VladislavTsurikov.RendererStack.Runtime.Core.RendererSystem.Renderer;

namespace VladislavTsurikov.RendererStack.Editor.Sectorize
{
    public class SectorizeMenu : RendererMenu
    {
        public override void ShowGenericMenu(GenericMenu menu, Renderer renderer)
        {
            Runtime.Sectorize.Sectorize sectorize = (Runtime.Sectorize.Sectorize)renderer;

            menu.AddItem(new GUIContent("Debug/All Cells"), sectorize.DebugAllCells, ContextMenuUtility.ContextMenuCallback, new Action(() => 
                sectorize.DebugAllCells = !sectorize.DebugAllCells));
            
            menu.AddItem(new GUIContent("Debug/Visible Cells"), sectorize.DebugVisibleCells, ContextMenuUtility.ContextMenuCallback, new Action(() => 
                sectorize.DebugVisibleCells = !sectorize.DebugVisibleCells));
        }
    }
}
#endif