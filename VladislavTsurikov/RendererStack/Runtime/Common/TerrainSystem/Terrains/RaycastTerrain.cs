using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using VladislavTsurikov.OdinSerializer.Core.Misc;
using VladislavTsurikov.Utility.Runtime;

namespace VladislavTsurikov.RendererStack.Runtime.Common.TerrainSystem
{
    [Serializable]
    [TerrainHelper(typeof(Area), "Raycast Terrain")]
    public class RaycastTerrain : TerrainHelper
    {
        [OdinSerialize] 
        private Area _area;

        public override void Init()
        {
            _area = (Area)Target;
        }

        public override Bounds GetTerrainBounds()
        {
            return _area.AreaBounds;
        }

        public override JobHandle SetCellHeight(NativeArray<Bounds> cellBoundsList, float minHeightCells,
            Rect cellBoundsRect, JobHandle dependsOn = default(JobHandle))
        {
            Bounds bounds = GetTerrainBounds();

            Rect terrainRect = RectExtension.CreateRectFromBounds(bounds);
            if (!cellBoundsRect.Overlaps(terrainRect)) return dependsOn;

            MeshCellSampleJob raycastTerranCellSampleJob = new MeshCellSampleJob
            {
                CellBoundsList = cellBoundsList,
                TerrainMinHeight = bounds.center.y - bounds.extents.y,
                TerrainMaxHeight = bounds.center.y + bounds.extents.y,
                TerrainRect = terrainRect
            };

            JobHandle handle = raycastTerranCellSampleJob.Schedule(cellBoundsList.Length, 32, dependsOn);
            return handle;
        }

        public override void RefreshData()
        {
        }

        public override void OnDisable()
        {
        }
    }
}