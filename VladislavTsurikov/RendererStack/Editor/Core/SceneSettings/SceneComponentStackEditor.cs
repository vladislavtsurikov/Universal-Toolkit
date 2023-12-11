#if UNITY_EDITOR
using System.Linq;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.RendererStack.Runtime.Core;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Attributes;

namespace VladislavTsurikov.RendererStack.Editor.Core.SceneSettings
{
    public class SceneComponentStackEditor : IMGUIComponentStackEditor<SceneComponent, IMGUIElementEditor>
    {
        public SceneComponentStackEditor(AdvancedComponentStack<SceneComponent> stack) : base(stack)
        {
        }
        
        protected override void OnIMGUIComponentStackGUI()
        {
            AddSceneComponentsAttribute addSceneComponentsAttribute = 
                (AddSceneComponentsAttribute)RendererStackManager.Instance.RendererStack.SelectedElement.GetType().GetAttribute(typeof(AddSceneComponentsAttribute));
            
            DrawElements(addSceneComponentsAttribute.Types.ToList());
        }
    }
}
#endif