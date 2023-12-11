using System.Collections.Generic;
using UnityEngine;

namespace VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.RenderModelData
{
    public class LOD 
    {
        public Mesh Mesh;
        public List<Material> Materials = new List<Material>();
        public MaterialPropertyBlock MaterialPropertyBlock;
        public MaterialPropertyBlock ShadowMaterialPropertyBlock;
        public float Distance = 0;
    }
}