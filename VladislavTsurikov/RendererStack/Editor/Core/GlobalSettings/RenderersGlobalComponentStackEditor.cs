using System;
using UnityEditor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.RendererStack.Runtime.Core;
using VladislavTsurikov.RendererStack.Runtime.Core.GlobalSettings;

namespace VladislavTsurikov.RendererStack.Editor.Core.GlobalSettings
{
    public class RenderersGlobalComponentStackEditor : IMGUIComponentStackEditor<RendererGlobalComponentStack, RendererGlobalComponentStackEditor>
    {
        public RenderersGlobalComponentStackEditor(RenderersGlobalComponentStack stack) : base(stack)
        {
        }

        protected override void OnIMGUIComponentStackGUI()
        {
            EditorGUI.BeginChangeCheck();
            
            var stackEditor = GetRendererGlobalComponentStackEditor(RendererStackManager.Instance.RendererStack.SelectedElement.GetType());

            stackEditor?.OnGUI();
            
            if(EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(Runtime.Core.GlobalSettings.GlobalSettings.Instance);
            }
        }

        private static IMGUIElementEditor GetRendererGlobalComponentStackEditor(Type rendererType)
        {
            foreach (var item in Runtime.Core.GlobalSettings.GlobalSettings.Instance.RenderersGlobalComponentStackEditor.Editors)
            {
                RendererGlobalComponentStack stack = (RendererGlobalComponentStack)item.Target;
                
                if (stack.RendererType == rendererType)
                {
                    return item;
                }
            }

            return null;
        }
    }
}