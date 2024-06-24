using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.PrototypeSettings;

namespace VladislavTsurikov.RendererStack.Runtime.Common.PrototypeSettings
{
    [MenuItem("Shadow")]
    public class Shadow : PrototypeComponent
    {
        [SerializeField]
        private float _shadowDistance = 4000;
        
        public float ShadowDistance
        {
            get => _shadowDistance;
            set
            {
                if(value < 0)
                {
                    _shadowDistance = 0;
                }
                else
                {
                    _shadowDistance = value;
                }
            }
        }

        public float[] ShadowLODMap = new float[] {
            0, 4, 0, 0,
            1, 5, 0, 0,
            2, 6, 0, 0,
            3, 7, 0, 0};
    }
}