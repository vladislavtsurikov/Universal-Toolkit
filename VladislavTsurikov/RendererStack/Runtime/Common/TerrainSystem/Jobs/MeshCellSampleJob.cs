using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using VladislavTsurikov.Utility.Runtime.Extensions;

namespace VladislavTsurikov.RendererStack.Runtime.Common.TerrainSystem.Jobs
{
#if RENDERER_STACK_BURST
    [BurstCompile(CompileSynchronously = true)]
#endif
    public struct MeshCellSampleJob : IJobParallelFor
    {
        public NativeArray<Bounds> CellBoundsList;
        public Rect TerrainRect;
        public float TerrainMinHeight;
        public float TerrainMaxHeight;

        public void Execute(int index)
        {
            Bounds cellBounds = CellBoundsList[index];
            Rect cellRect = RectExtension.CreateRectFromBounds(cellBounds);
            if (!TerrainRect.Overlaps(cellRect)) return;

            float minHeight;
            float maxHeight = cellBounds.center.y + cellBounds.extents.y;

            if (cellBounds.center.y < 99999)
            {
                minHeight = TerrainMinHeight;
            }
            else
            {
                minHeight = cellBounds.center.y - cellBounds.extents.y;
            }

            if (TerrainMinHeight < minHeight) minHeight = TerrainMinHeight;
            if (TerrainMaxHeight > maxHeight) maxHeight = TerrainMaxHeight;

            float centerY = (maxHeight + minHeight) / 2f;
            float height = maxHeight - minHeight;
            cellBounds =
                new Bounds(new Vector3(cellBounds.center.x, centerY, cellBounds.center.z),
                    new Vector3(cellBounds.size.x, height, cellBounds.size.z));
            CellBoundsList[index] = cellBounds;
        }
    }
}