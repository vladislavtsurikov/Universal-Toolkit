#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.RendererStack.Editor.Core.RendererSystem;
using VladislavTsurikov.RendererStack.Runtime.Core.RendererSystem;
using VladislavTsurikov.RendererStack.Runtime.Core.RenderManager.RenderModes.GPUInstancedIndirect.GPUISupport;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.RendererData;
using GUIUtility = VladislavTsurikov.Utility.Runtime.GUIUtility;

namespace VladislavTsurikov.RendererStack.Editor.TerrainObjectRenderer
{
    public class TerrainObjectRendererMenu : RendererMenu
    {
        public override void ShowGenericMenu(GenericMenu menu, CustomRenderer customRenderer)
        {
            Runtime.TerrainObjectRenderer.TerrainObjectRenderer terrainObjectRenderer = (Runtime.TerrainObjectRenderer.TerrainObjectRenderer)customRenderer;
            
            menu.AddItem(new GUIContent("Regenerate Instanced Indirect Shaders"), false, GUIUtility.ContextMenuCallback, 
                new Action(() => { GPUInstancedIndirectShaderStack.Instance.RegenerateShaders(); })); 

            menu.AddItem(new GUIContent("Terrain Cells Occlusion Culling/Refresh Cells"), false, GUIUtility.ContextMenuCallback, 
                new Action(TerrainObjectRendererData.RefreshAllCells)); 
            
            menu.AddSeparator ("");

            menu.AddItem(new GUIContent("Debug/Terrain Cells Occlusion Culling/All Cells"), terrainObjectRenderer.DebugAllCells, GUIUtility.ContextMenuCallback, new Action(() => 
                terrainObjectRenderer.DebugAllCells = !terrainObjectRenderer.DebugAllCells));
            
            menu.AddItem(new GUIContent("Debug/Terrain Cells Occlusion Culling/Visible Cells"), terrainObjectRenderer.DebugVisibleCells, GUIUtility.ContextMenuCallback, new Action(() => 
                terrainObjectRenderer.DebugVisibleCells = !terrainObjectRenderer.DebugVisibleCells));
        }
    }
}
#endif