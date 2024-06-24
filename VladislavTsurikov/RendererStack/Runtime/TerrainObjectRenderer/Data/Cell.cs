using System;
using UnityEngine;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.OdinSerializer.Core.Misc;
using VladislavTsurikov.QuadTree.Runtime;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data.ColliderSystem;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data.RendererData;
using VladislavTsurikov.Utility.Runtime;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data
{
    [Serializable]
    public class Cell : IHasRect
    {
        [NonSerialized]
        private TerrainObjectRendererData _sceneData;

        [NonSerialized]
        public TerrainObjectRendererCollider TerrainObjectRendererCollider;
        
        [OdinSerialize]
        public Bounds Bounds;
        [OdinSerialize]
        public PrototypeRenderDataStack PrototypeRenderDataStack = new PrototypeRenderDataStack();
        [OdinSerialize]
        public int Index;

        [NonSerialized]
        public AABB InitialObjectsAABB;

        public Cell(Bounds bounds)
        {
            Bounds = bounds;
        }

        public Cell(Rect rectangle, TerrainObjectRendererData sceneData, SelectionData selectionData)
        {
            Bounds = RectExtension.CreateBoundsFromRect(rectangle, -100000);
            Setup(sceneData, selectionData);
        }

        public Rect Rectangle
        {
            get => RectExtension.CreateRectFromBounds(Bounds);
            set => Bounds = RectExtension.CreateBoundsFromRect(value);
        }

        public void Setup(TerrainObjectRendererData sceneData, SelectionData selectionData)
        {
            _sceneData = sceneData;

            PrototypeRenderDataStack.Setup(selectionData);
            
            if (TerrainObjectRendererCollider == null)
            {
                TerrainObjectRendererCollider = new TerrainObjectRendererCollider(sceneData.SceneDataManager, this, PrototypeRenderDataStack);
            }
            else
            {
                TerrainObjectRendererCollider.Setup();
            }
        }

        public void ChangeNodeSizeIfNecessary(TerrainObjectCollider instantObject)
        {
            _sceneData.ChangeNodeSizeIfNecessary(instantObject, this);
        }

        public AABB GetObjectsAABB()
        {
            return TerrainObjectRendererCollider.GetAABB();
        }

        public Bounds GetObjectBounds()
        {
            AABB aabb = GetObjectsAABB();
            Bounds cellObjectBounds = new Bounds(aabb.Center, aabb.Size);
                        
            Bounds bounds = Bounds;
            bounds.Encapsulate(cellObjectBounds);

            return bounds;
        }
    }
}
