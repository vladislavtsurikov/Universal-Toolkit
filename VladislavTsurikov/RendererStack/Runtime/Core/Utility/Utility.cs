using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Core.Extensions;
using VladislavTsurikov.RendererStack.Runtime.Common.GlobalSettings;
using VladislavTsurikov.RendererStack.Runtime.Common.PrototypeSettings;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.PrototypeSettings;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.RenderModelData;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Camera;

namespace VladislavTsurikov.RendererStack.Runtime.Core.Utility
{
    public static class Utility 
    {
        public static bool IsInLayer(int layerMask, int layer)
        {
            return layerMask == (layerMask | (1 << layer));
        }

        public static float[] GetLODDistances(RenderModel renderModel, float lodBias, float maxDistance, bool forInstancedIndirect = true)
        {
            float[] lodDistances = new float[] 
            {
                1000, 1000, 1000, 1000,
                1000, 1000, 1000, 1000,
                1000, 1000, 1000, 1000,
                1000, 1000, 1000, 1000 
            };

            for (int i = 0; i < renderModel.LODs.Count; i++)
            {
                int index = i;

                if(forInstancedIndirect)
                {
                    index = i * 4;
                    if (i >= 4)
                    {   
                        index = (i - 4) * 4 + 1;
                    }
                }
                
                if(i == renderModel.LODs.Count - 1)
                {   
                    lodDistances[index] = maxDistance;
                }   
                else
                {
                    lodDistances[index] = renderModel.LODs[i + 1].Distance * lodBias;
                }
            }

            return lodDistances;
        }

        public static float GetMaxDistance(Type rendererType, VirtualCamera virtualCamera, DistanceCulling distanceCulling)
        {
            if (!distanceCulling.IsValid())
            {
                return 0;
            }
            
            Quality quality = (Quality)GlobalSettings.GlobalSettings.Instance.GetElement(typeof(Quality), rendererType);
            
            float maxDistance = distanceCulling.MaxDistance;

            maxDistance = Mathf.Min(maxDistance, quality.MaxRenderDistance);
            maxDistance = Mathf.Min(maxDistance, virtualCamera.Camera.farClipPlane);
            
            return maxDistance;
        }
    }
}