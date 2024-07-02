using System.Linq;
using System.Reflection;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ReflectionUtility.Runtime;
using VladislavTsurikov.RendererStack.Runtime.Core.RendererSystem;

namespace VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings
{
    public class SceneComponentStack : ComponentStackOnlyDifferentTypes<SceneComponent>
    {
        protected override void OnCreateElements()
        {
            foreach (var rendererType in AllTypesDerivedFrom<Renderer>.TypeList)
            {
                AddSceneComponentsAttribute addComponentsAttribute = rendererType.GetAttribute<AddSceneComponentsAttribute>();

                if (addComponentsAttribute == null)
                {
                    continue;
                }
                
                CreateElementIfMissingType(addComponentsAttribute.Types);
            }
        }

        public override void OnRemoveInvalidElements()
        {
            for (int i = ElementList.Count - 1; i >= 0; i--)
            {
                bool find = false;
                
                foreach (var rendererType in AllTypesDerivedFrom<Renderer>.TypeList)
                {
                    AddSceneComponentsAttribute addComponentsAttribute = 
                        (AddSceneComponentsAttribute)rendererType.GetAttribute(typeof(AddSceneComponentsAttribute));

                    if (addComponentsAttribute == null)
                    {
                        continue;
                    }

                    if (addComponentsAttribute.Types.Contains(ElementList[i].GetType()))
                    {
                        find = true;
                        break;
                    }
                }

                if (!find)
                {
                    Remove(i);
                }
            }
        }

#if UNITY_EDITOR
        public void DrawDebug(Renderer renderer)
        {
            if (renderer == null)
            {
                return;
            }
            
            AddSceneComponentsAttribute addSceneComponentsAttribute = (AddSceneComponentsAttribute)renderer.GetType().GetCustomAttribute(typeof(AddSceneComponentsAttribute));

            foreach (SceneComponent setting in _elementList.Where(setting => setting != null))
            {
                if(addSceneComponentsAttribute.Types.Contains(setting.GetType()))
                {
                    if(setting.Selected)
                    {
                        setting.OnSelectedDrawGizmos();
                    }

                    setting.OnDrawGizmos();
                }
            }
        }
#endif
    }
}