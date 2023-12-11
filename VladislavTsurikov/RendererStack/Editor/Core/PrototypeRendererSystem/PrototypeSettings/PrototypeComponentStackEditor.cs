#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Editor;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList.Attributes;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.PrototypeSettings;

namespace VladislavTsurikov.RendererStack.Editor.Core.PrototypeRendererSystem.PrototypeSettings
{
    public class PrototypeComponentStackEditor : ReorderableListStackEditor<PrototypeComponent, PrototypeComponentEditor>
    {
        private readonly ComponentStackOnlyDifferentTypes<PrototypeComponent> _componentStackOnlyDifferentTypes;
        
        public PrototypeComponentStackEditor(ComponentStackOnlyDifferentTypes<PrototypeComponent> stack) : 
            base(new GUIContent(""), stack, true)
        {
            _componentStackOnlyDifferentTypes = stack;
        }
    }
}
#endif