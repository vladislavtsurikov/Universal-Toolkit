using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.RendererData.ColliderSystem;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.RendererData.RendererDataSystem;
using VladislavTsurikov.SceneDataSystem.Runtime;
using VladislavTsurikov.SceneDataSystem.Runtime.Utility;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.RendererData
{
    public partial class TerrainObjectRendererData 
    {
        public static void AddInstance(TerrainObjectInstance instance, string sectorLayerTag)
        {
            List<SceneDataManager> sceneDataManagers = FindSceneDataManager.OverlapPosition(instance.Position, sectorLayerTag, false);
                
            if (sceneDataManagers.Count != 0)
            {
                AddInstance(instance, sceneDataManagers[0]);
            }
        }

        private static void AddInstance(TerrainObjectInstance instance, SceneDataManager sceneDataManager)
        {
            TerrainObjectRendererData terrainObjectRendererData = (TerrainObjectRendererData)sceneDataManager.SceneDataStack.GetElement(typeof(TerrainObjectRendererData));

            if(terrainObjectRendererData == null)
            {
                return;
            }
            
            Rect positionRect = new Rect(new Vector2(instance.Position.x, instance.Position.z), Vector2.zero);

            List<Cell> overlapCellList = new List<Cell>();                 
            terrainObjectRendererData._cellQuadTree.Query(positionRect, overlapCellList);
            
            if(overlapCellList.Count == 0)
            {
                return;
            }

            Cell cell = overlapCellList[0];

            PrototypeRendererData prototypeRendererData = cell.PrototypeRenderDataStack.GetPrototypeRenderData(instance.Proto.ID);

            if (prototypeRendererData == null)
            {
                prototypeRendererData = new PrototypeRendererData(instance.Proto.ID);
                cell.PrototypeRenderDataStack.PrototypeRenderDataList.Add(prototypeRendererData);
            }

            TerrainObjectCollider instantObjectCollider = cell.TerrainObjectRendererCollider.RegisterObject(instance, cell, prototypeRendererData, sceneDataManager);

            if(instantObjectCollider != null)
            {
                prototypeRendererData.AddPersistentData(instance);
                terrainObjectRendererData.ChangeNodeSizeIfNecessary(instantObjectCollider, cell);
            }
        }
        
        public static void AddInstances(List<TerrainObjectCollider> instantObjects, string sectorLayerTag)
        {
            foreach (var instance in instantObjects)
            {
                AddInstance(instance.Instance, sectorLayerTag);
            }
        }
    }
}