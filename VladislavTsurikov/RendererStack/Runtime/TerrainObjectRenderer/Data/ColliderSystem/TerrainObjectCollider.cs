using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.Math.Runtime;
using Mesh = VladislavTsurikov.ColliderSystem.Runtime.Mesh;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data.ColliderSystem
{
    public class TerrainObjectCollider : ColliderObject 
    {
        public TerrainObjectInstance Instance => (TerrainObjectInstance)Obj;
        public PathToTerrainObjectCollider PathToTerrainObjectCollider => (PathToTerrainObjectCollider)PathToColliderObject;

        public TerrainObjectCollider(object obj) : base(obj)
        {
            
        }

        public override bool IsRendererEnabled()
        {
            return true;
        }

        public override OBB GetOBB()
        {
            Mesh mesh = GetMesh();
            
            return mesh == null ? new OBB() : ObjectBounds.CalcWorldOBB(mesh.UnityMesh, GetMatrix());
        }

        public override AABB GetAABB()
        {
            Mesh mesh = GetMesh();

            return mesh == null ? new AABB() : ObjectBounds.CalcWorldAABB(mesh.UnityMesh, GetMatrix());
        }

        public override Matrix4x4 GetMatrix()
        {
            return Instance.GetMatrix();
        }

        public override Mesh GetMesh()
        {
            if (Instance.Proto.RenderModel.LODs.Count == 0)
            {
                return null;
            }
            
            UnityEngine.Mesh mesh = Instance.Proto.RenderModel.LODs[0].Mesh;
            if (mesh != null)
            {
                return MeshStack.GetEditorMesh(mesh);
            }

            return null;
        }

        public override int GetLayer()
        {
            return Instance.Proto.Prefab.layer;
        }

        public override bool IsValid()
        {
            return true;
        }

        public override GameObject GetPrefab()
        {
            return Instance.Proto.Prefab;
        }
        
        protected override void SetPathToObjectCollider(List<object> pathDatas)
        {
            PathToColliderObject = new PathToTerrainObjectCollider(pathDatas);
            Instance.TerrainObjectCollider = this;
        }
    }
}