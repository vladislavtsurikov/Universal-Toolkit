using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.SceneDataSystem.Runtime;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data
{
    public partial class TerrainObjectRendererData 
    {
        public List<ColliderObject> OverlapBox(Vector3 boxCenter, Vector3 boxSize, Quaternion boxRotation, ObjectFilter objectFilter, SceneData sceneData, bool quadTree, bool checkObbIntersection)
        {
            var overlappedObjects = new List<ColliderObject>();

            foreach (Cell cell in OverlapCellBox(boxCenter,  boxSize, boxRotation, quadTree))
            {
                overlappedObjects.AddRange(cell.TerrainObjectRendererCollider.OverlapBox(boxCenter, boxSize, boxRotation, objectFilter, sceneData, cell, quadTree, checkObbIntersection));
            }

            return overlappedObjects;
        }

        public List<ColliderObject> OverlapSphere(Vector3 sphereCenter, float sphereRadius, ObjectFilter objectFilter, SceneData sceneData, bool quadTree, bool checkObbIntersection)
        {
            var overlappedObjects = new List<ColliderObject>();
            
            foreach (Cell cell in OverlapCellSphere(sphereCenter, sphereRadius, quadTree))
            {
                overlappedObjects.AddRange(cell.TerrainObjectRendererCollider.OverlapSphere(sphereCenter, sphereRadius, objectFilter, sceneData, cell, quadTree, checkObbIntersection));
            }

            return overlappedObjects;
        }

        public List<Cell> OverlapCellBox(Vector3 boxCenter, Vector3 boxSize, Quaternion boxRotation, bool quadTree)
        {
            List<Cell> overlapCellList = new List<Cell>();    

            if (quadTree)
            {
                Vector2 position = new Vector2(boxCenter.x - boxSize.x / 2, boxCenter.z - boxSize.z / 2);
                Rect selectedAreaRect = new Rect(position, new Vector2(boxSize.x, boxSize.z));   
                
                _cellQuadTree.Query(selectedAreaRect, overlapCellList);
            }
            else
            {
                overlapCellList = BVHCellTree.OverlapCellsBox(boxCenter, boxSize, boxRotation);
            }

            return overlapCellList;
        }

        public List<Cell> OverlapCellSphere(Vector3 sphereCenter, float sphereRadius, bool quadTree)
        {
            List<Cell> overlapCellList = new List<Cell>();    
            
            if (quadTree)
            {
                Vector2 position = new Vector2(sphereCenter.x - sphereRadius, sphereCenter.z - sphereRadius);
                Rect selectedAreaRect = new Rect(position, new Vector2(sphereRadius * 2, sphereRadius * 2));   
                
                _cellQuadTree.Query(selectedAreaRect, overlapCellList);
            }
            else
            {
                overlapCellList = BVHCellTree.OverlapCellsSphere(sphereCenter, sphereRadius);
            }

            return overlapCellList;
        }

        public RayHit Raycast(Ray ray, ObjectFilter objectFilter)
        {
            List<RayHit> allObjectHits = RaycastAll(ray, objectFilter);
            RayHit closestObjectHit = allObjectHits.Count != 0 ? allObjectHits[0] : null;
            return closestObjectHit;
        }

        public List<RayHit> RaycastAll(Ray ray, ObjectFilter objectFilter)
        {
            var cells = BVHCellTree.RaycastAll(ray);

            var rayHits = new List<RayHit>();

            foreach (var cell in cells)
            {
                rayHits.AddRange(cell.TerrainObjectRendererCollider.RaycastAll(ray, objectFilter, this, cell)); 
            }

            return rayHits;
        }
    }
}