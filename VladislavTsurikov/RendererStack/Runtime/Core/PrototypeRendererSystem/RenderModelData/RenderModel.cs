using System.Collections.Generic;
using UnityEngine;

namespace VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.RenderModelData
{
    public class RenderModel
    {
        public GameObject Prefab;
        public Vector3 MultiplySize;
        public float BoundingSphereRadius;
        public List<LOD> LODs = new List<LOD>();

        public RenderModel(GameObject prefab)
        {            
            Prefab = prefab;
        }

        public void SetLODFadeKeyword(bool useLodFade)
        {
            foreach (LOD lod in LODs)
            {
                foreach (Material mat in lod.Materials)
                {
                    if(useLodFade)
                    {
                        mat.EnableKeyword("LOD_FADE_CROSSFADE"); 
                    }
                    else
                    {
                        mat.DisableKeyword("LOD_FADE_CROSSFADE"); 
                    }
                }
            }
        }
    }
}