using UnityEngine;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.QuadTree.Runtime;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.SelectionDatas;
using VladislavTsurikov.Utility.Runtime.Extensions;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.RendererData.ColliderSystem
{
    public class ColliderCell : IHasRect
    {
        private TerrainObjectRendererCollider _terrainObjectRendererCollider;

        public PrototypeBVHObjectTreeStack PrototypeBVHObjectTreeStack = new PrototypeBVHObjectTreeStack();

        public Bounds Bounds;

        public ColliderCell(Bounds bounds)
        {
            Bounds = bounds;
        }

        public Rect Rectangle
        {
            get => RectExtension.CreateRectFromBounds(Bounds);
            set => Bounds = RectExtension.CreateBoundsFromRect(value);
        }

        public void Prepare(TerrainObjectRendererCollider terrainObjectRendererCollider, SelectionData selectionData)
        {
            _terrainObjectRendererCollider = terrainObjectRendererCollider; 

            PrototypeBVHObjectTreeStack.PreparePrototypeRenderCount(selectionData); 
        }

        public void ChangeNodeSizeIfNecessary(TerrainObjectCollider terrainObjectCollider)
        {
            _terrainObjectRendererCollider.ChangeNodeSizeIfNecessary(terrainObjectCollider, this);
        }

        public AABB GetObjectsAABB()
        {
            AABB currentAABB = new AABB(Bounds);

            foreach (PrototypeBVHObjectTree prototypeBVHObjectTree in PrototypeBVHObjectTreeStack.PrototypeBVHObjectTreeList)
            {
                AABB prototypeObjectsAABB = prototypeBVHObjectTree.BVHObjectTree.Tree.GetAABB();

                if(prototypeObjectsAABB.IsValid)
                {   
                    currentAABB.Encapsulate(prototypeObjectsAABB); 
                }
            }

            return currentAABB;
        }
    }
}