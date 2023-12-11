#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Editor.Attributes;
using VladislavTsurikov.ComponentStack.Runtime;
using Component = VladislavTsurikov.ComponentStack.Runtime.Component;

namespace VladislavTsurikov.ComponentStack.Editor
{
    public abstract class ComponentStackEditor<T, N> 
        where T: Component
        where N: ElementEditor
    {
        public ComponentStack<T> Stack {get;}
        protected List<N> Editors;
        
        public N SelectedEditor
        {
            get
            {
                return Editors.FirstOrDefault(t => ((Component)t.Target).Selected);
            }
        }

        protected ComponentStackEditor(ComponentStack<T> stack)
        {
            Stack = stack;
            Editors = new List<N>();
            RefreshEditors();
        }
        
        protected virtual void Create(T settings, int index = -1)
        {
            var settingsType = settings.GetType();

            if (settingsType.GetAttribute(typeof(DontDrawAttribute)) != null)
            {
                return;
            }

            if (AllEditorTypes<T>.Types.TryGetValue(settingsType, out var editorType))
            {
                var editor = (N)Activator.CreateInstance(editorType);
                editor.Init(settings);

                if (index < 0)
                {
                    Editors.Add(editor);
                }
                else
                {
                    Editors[index] = editor;
                }
            }
        }

        public void RefreshEditors()
        {
            Editors = new List<N>();

            Stack.RemoveInvalidElements();
            
            foreach (var t in Stack.ElementList)
            {
                Create(t);
            }
        }
    }
}
#endif