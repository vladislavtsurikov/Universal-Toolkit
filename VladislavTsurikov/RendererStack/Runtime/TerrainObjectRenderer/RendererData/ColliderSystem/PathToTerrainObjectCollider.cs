using System.Collections.Generic;
using VladislavTsurikov.BVH.Runtime;
using VladislavTsurikov.ColliderSystem.Runtime.Scene;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.RendererData.RendererDataSystem;
using VladislavTsurikov.SceneDataSystem.Runtime;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.RendererData.ColliderSystem
{
    public class PathToTerrainObjectCollider : PathToColliderObject
    {
        public readonly SceneDataManager SceneDataManager;
        public readonly PrototypeRendererData PrototypeRendererData;
        public readonly ColliderCell ColliderCell; 
        public readonly Cell RenderCell;
        public readonly BVHNodeAABB<TerrainObjectCollider> LeafNode;
        public readonly BVHObjectTree<TerrainObjectCollider> BVHObjectTree;
        
        public PathToTerrainObjectCollider(List<object> datas)
        {
            foreach (var item in datas)
            { 
                switch (item)
                {
                    case SceneDataManager sceneDataManager:
                        SceneDataManager = sceneDataManager;
                        break;
                    case PrototypeRendererData data:
                        PrototypeRendererData = data;
                        break;
                    case ColliderCell cell:
                        ColliderCell = cell;
                        break;
                    case Cell cell:
                        RenderCell = cell;
                        break;
                    case BVHObjectTree<TerrainObjectCollider> tree:
                        BVHObjectTree = tree;
                        break;
                    case BVHNodeAABB<TerrainObjectCollider> node:
                        LeafNode = node;
                        break;
                }
            }
        }
    }
}