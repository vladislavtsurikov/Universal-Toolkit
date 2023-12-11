using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.PrototypeSettings;

namespace VladislavTsurikov.RendererStack.Runtime.Common.PrototypeSettings.Components
{
    public enum GetAdditionalShadow 
    {
        None,
        MinCullingDistance,
        IncreaseBoundingSphere,
        DirectionLightShadowVisible
    }
    
    [MenuItem("Frustum Culling Settings")]
    public class FrustumCulling : PrototypeComponent
    {
        [SerializeField]
        private float _minCullingDistance = 50;
        public float MinCullingDistance 
        {
            get => _minCullingDistance;
            set
            {
                if(value < 0)
                {
                    _minCullingDistance = 0;
                }
                else
                {
                    _minCullingDistance = value;
                }
            }
        }

        public GetAdditionalShadow GetAdditionalShadow = GetAdditionalShadow.DirectionLightShadowVisible;
        public float IncreaseBoundingSphere = 7f;
        public float IncreaseShadowsBoundingSphere;
    }
}