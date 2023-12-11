using System.Collections.Generic;
using System.Linq;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.RendererStack.Runtime.Core.RendererSystem.Attributes;
using VladislavTsurikov.SceneDataSystem.Runtime;
using VladislavTsurikov.SceneDataSystem.Runtime.Utility;

namespace VladislavTsurikov.RendererStack.Runtime.Core.RendererSystem
{
    public abstract class CustomRenderer : ComponentStack.Runtime.Component
    {
        public bool ForceUpdateRendererData = false;

        protected override void SetupElement(object[] args = null)
        {
            SetupSceneData(); 
            SetupRenderer();
        }
        
        private void SetupSceneData()
        {
            AddSceneDataAttribute[] addSceneDataAttributes = GetType().GetAttributes<AddSceneDataAttribute>().ToArray();

            List<SceneDataManager> sceneDataManagers = SceneDataManagerUtility.GetAllSceneDataManager();
            
            foreach (var addSceneDataAttribute in addSceneDataAttributes)
            {
                foreach (var type in addSceneDataAttribute.Types)
                {
                    RequiredSceneData.AddRequiredType(type);
                    
                    foreach (var sceneDataManager in sceneDataManagers)
                    {
                        sceneDataManager.SceneDataStack.SetupElement(type, true);
                    }
                }
            }
        }

        protected override void OnCreate()
        {
            GlobalSettings.GlobalSettings.Instance.Setup();
        }

        protected virtual void SetupRenderer(){}
        public virtual void Render(){}
        public virtual void DrawDebug(){}
        public virtual void CheckChanges(){}
    }
}