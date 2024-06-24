using UnityEngine;
using UnityEngine.Rendering;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.RendererStack.Runtime.Core.GlobalSettings;

namespace VladislavTsurikov.RendererStack.Runtime.Common.GlobalSettings
{
    [MenuItem("Quality")]
    public class Quality : GlobalComponent
    {
        [SerializeField]
        private float _maxRenderDistance = 8000;
        
        public float LODBias = 1;
        public bool IsShadowCasting = true;
        
        public float MaxRenderDistance
        {
            get => _maxRenderDistance;
            set
            {
                if(value < 0)
                {
                    _maxRenderDistance = 0;
                }
                else
                {
                    _maxRenderDistance = value;
                }
            }
        }

        public ShadowCastingMode GetShadowCastingMode()
        {
            return IsShadowCasting ? ShadowCastingMode.On : ShadowCastingMode.Off;
        }
    }
}