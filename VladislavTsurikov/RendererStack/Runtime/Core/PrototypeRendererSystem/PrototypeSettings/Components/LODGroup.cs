using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.RenderModelData;

namespace VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.PrototypeSettings
{
    [Name("LOD Group")]
    [DontShowInAddMenu]
    [PersistentComponent]
    public class LODGroup : PrototypeComponent
    { 
        [SerializeField]
        private bool _enabledLODFade;
        
        public bool EnabledLODFade => _enabledLODFade;
        
        public float LODDistanceRandomOffset;
        public float LODBias = 1;
        public float LodFadeTransitionDistance = 10;
        public bool LodFadeForLastLOD = true;
        

        public void SetLODFade(RenderModel renderModel, bool value)
        {
            if(value != _enabledLODFade)
    		{
    			renderModel.SetLODFadeKeyword(value);
                _enabledLODFade = value;
    		}
        }

        protected override bool IsDeletableComponent()
        {
            return false;
        }
        
        public override bool ShowActiveToggle()
        {
            return false;
        }
    }
}