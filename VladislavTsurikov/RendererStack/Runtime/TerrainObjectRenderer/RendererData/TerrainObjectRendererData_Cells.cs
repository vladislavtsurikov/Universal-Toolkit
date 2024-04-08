using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Profiling;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.QuadTree.Runtime;
using VladislavTsurikov.RendererStack.Runtime.Common.TerrainSystem;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Components.Camera;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.RendererData.ColliderSystem;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.RendererData.RendererDataSystem;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.RendererData.RendererDataSystem.Utility;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.SceneSettings.Components.Camera;
using VladislavTsurikov.SceneDataSystem.Runtime.Utility;
using VladislavTsurikov.Utility.Runtime.Extensions;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.RendererData
{
    public partial class TerrainObjectRendererData
    {
        private static List<Cell> cellsCached = new List<Cell>();
        
        public static List<Cell> GetAllVisibleCellList(VirtualCamera virtualCamera)
        {
            if (cellsCached == null)
            {
                cellsCached = new List<Cell>();
            }
            
            cellsCached.Clear();
            
            Profiler.BeginSample("GetAllVisibleCellList");
            foreach (TerrainObjectRendererData item in SceneDataStackUtility.GetAllSceneData<TerrainObjectRendererData>())
            {
                cellsCached.AddRange(item.GetVisibleCellList(virtualCamera));
            }
            Profiler.EndSample();

            return cellsCached;
        }
        
        public static List<Cell> GetCellsWithInstances(List<Cell> cellList, int protoID, out int totalInstanceCount)
        {
            totalInstanceCount = 0;
            List<Cell> currentCellList = new List<Cell>();

            foreach (Cell cell in cellList)
            {
                PrototypeRendererData prototypeRendererData = cell.PrototypeRenderDataStack.GetPrototypeRenderData(protoID);
                int count = prototypeRendererData?.InstanceList.Count ?? 0;

                if(count != 0)
                {
                    currentCellList.Add(cell);
                    totalInstanceCount += count;
                } 
            }

            return currentCellList;
        }

        public static void RefreshAllCells()
        {
            List<TerrainObjectCollider> largeObjectInstances = new List<TerrainObjectCollider>();
            
            SceneDataStackUtility.Setup<TerrainObjectRendererData>(true);
            
            foreach (TerrainObjectRendererData item in SceneDataStackUtility.GetAllSceneData<TerrainObjectRendererData>())
            {
                largeObjectInstances.AddRange(item.GetAllInstances());
                
                item.RefreshCells();
            }

            foreach (var instance in largeObjectInstances)
            {
                AddInstance(instance.Instance,  Sectorize.Sectorize.GetSectorLayerTag());
            }
        }
        
        private static List<Cell> visibleCellListCached = new List<Cell>();

        public List<Cell> GetVisibleCellList(VirtualCamera virtualCamera)
        {
            TerrainObjectRendererCameraSettings terrainObjectRendererCameraSettings = (TerrainObjectRendererCameraSettings)virtualCamera.CameraComponentStack.GetElement(typeof(TerrainObjectRendererCameraSettings));

            if (visibleCellListCached == null)
            {
                visibleCellListCached = new List<Cell>();
            }
            
            visibleCellListCached.Clear();
            
            if (terrainObjectRendererCameraSettings == null)
            {
                return visibleCellListCached;
            }

            Plane[] frustumPlaneArray = new Plane[6];

            GeometryUtility.CalculateFrustumPlanes(virtualCamera.Camera, frustumPlaneArray);
            
            float maxDistance = virtualCamera.GetMaxDistance(typeof(TerrainObjectRenderer));

            Vector3 selectedCameraPosition = virtualCamera.GetCameraPosition();

            float areaSize = maxDistance * 2 + _cellSize;

            Vector2 position = new Vector2(selectedCameraPosition.x - areaSize / 2f, selectedCameraPosition.z - areaSize / 2f);
            Rect selectedAreaRect = new Rect(position, new Vector2(areaSize, areaSize));

            if (_cellQuadTree == null)
            {
                return visibleCellListCached;
            }
            
            Profiler.BeginSample("QueryCells");
            
            //_cellQuadTree.Query(selectedAreaRect, visibleCellListCached);
            
            _cellQuadTree.Query(selectedAreaRect, cell =>
            {
                
                if(Vector3.Distance(selectedCameraPosition, cell.Bounds.center) > maxDistance + cell.Bounds.size.x)
                {
                    return true;
                }
                if(terrainObjectRendererCameraSettings.CameraCullingMode == CameraCullingMode.FrustumCulling)
                {
                    if(GeometryUtility.TestPlanesAABB(frustumPlaneArray, cell.GetObjectBounds()))
                    {
                        visibleCellListCached.Add(cell);
                    }
                }
                else
                {
                    visibleCellListCached.Add(cell);
                }
                
                visibleCellListCached.Add(cell);
                
                return true;
            });

            Profiler.EndSample();
            
            return visibleCellListCached;
        }

        public void RefreshCells()
        {
            ModifiedPrototypeRenderDataStack.Clear();
            
            TerrainManager terrainManager = SceneDataStackUtility.InstanceSceneData<TerrainManager>(SceneDataManager.Scene);
            SceneDataManager.SceneDataStack.SetupElement<TerrainManager>(true);

            CellList.Clear();
            BVHCellTree.Clear();

            if(terrainManager.Bounds.size == Vector3.zero)
            {
                return;
            }

            _cellSize = terrainManager.Bounds.size.x;

            while(_cellSize > ConstCellSize)
            {
                _cellSize /= 2;
            }

            if(_cellSize <= 0)
            {
                return;
            }
            
            _bounds = new Bounds(terrainManager.Bounds.center, terrainManager.Bounds.size);
            Bounds expandedBounds = _bounds;
            expandedBounds.Expand(new Vector3(_cellSize * 2f, 0, _cellSize * 2f));

            Rect expandedRect = RectExtension.CreateRectFromBounds(expandedBounds);

            _cellQuadTree = new QuadTree<Cell>(expandedRect); 
            
            int cellXCount = Mathf.CeilToInt(terrainManager.Bounds.size.x / _cellSize);
            int cellZCount = Mathf.CeilToInt(terrainManager.Bounds.size.z / _cellSize);

            Vector2 corner = new Vector2(terrainManager.Bounds.center.x - terrainManager.Bounds.size.x / 2f,
                terrainManager.Bounds.center.z - terrainManager.Bounds.size.z / 2f);

            for (int x = 0; x <= cellXCount - 1; x++)
            {
                for (int z = 0; z <= cellZCount - 1; z++)
                {
                    Cell cell = new Cell(new Rect(
                        new Vector2(_cellSize * x + corner.x, _cellSize * z + corner.y),
                        new Vector2(_cellSize, _cellSize)), this, TerrainObjectRenderer.Instance.SelectionData);
                    CellList.Add(cell);
                    cell.Index = CellList.Count - 1;
                    _cellQuadTree.Insert(cell);
                }
            }

            NativeArray<Bounds> cellBounds =
                new NativeArray<Bounds>(CellList.Count, Allocator.Persistent);
            for (int i = 0; i <= CellList.Count - 1; i++)
            {
                cellBounds[i] = CellList[i].Bounds;
            }

            float minBoundsHeight = terrainManager.Bounds.center.y - terrainManager.Bounds.extents.y;

            float minHeightCells = minBoundsHeight;

            JobHandle jobHandle = default;
            
            for (int i = 0; i <= terrainManager.TerrainHelperList.Count - 1; i++)
            {
                jobHandle = terrainManager.TerrainHelperList[i].SetCellHeight(cellBounds, minHeightCells, expandedRect, jobHandle);
            }

            jobHandle.Complete();

            for (int i = 0; i <= CellList.Count - 1; i++)
            {
                CellList[i].Bounds = cellBounds[i];
            }  

            cellBounds.Dispose();

            foreach (var cell in CellList)
            {
                BVHCellTree.RegisterObject(cell, new AABB(cell.Bounds));
                cell.TerrainObjectRendererCollider.RefreshCells(cell.Bounds);
            }
        }
    }
}