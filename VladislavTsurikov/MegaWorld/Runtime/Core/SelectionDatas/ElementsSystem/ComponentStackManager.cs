using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem.Utility;
using VladislavTsurikov.OdinSerializer.Core.Misc;
using Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;
#if UNITY_EDITOR
using UnityEditor;
using VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.ElementsSystem;
#endif

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem
{
    [Serializable]
    public class ComponentStackManager
    {
        [OdinSerialize] 
        private GeneralComponentStack _generalGeneralComponentStack;
        [OdinSerialize] 
        private ToolsComponentStack _toolsComponentStack;

        private Type _prototypeType;
        private Type _addGeneralElementsAttribute;
        private Type _addElementsAttribute;

        public GeneralComponentStack GeneralComponentStack => _generalGeneralComponentStack;
        public ToolsComponentStack ToolsComponentStack => _toolsComponentStack;
        
#if UNITY_EDITOR
        [NonSerialized] 
        private IMGUIComponentStackEditor<Component, IMGUIElementEditor> _generalComponentStackEditor;
        public IMGUIComponentStackEditor<Component, IMGUIElementEditor> GeneralComponentStackEditor
        {
            get
            {
                if (_generalComponentStackEditor == null)
                {
                    _generalComponentStackEditor = new IMGUIComponentStackEditor<Component, IMGUIElementEditor>(_generalGeneralComponentStack);
                }

                return _generalComponentStackEditor;
            }
        }
        
        [NonSerialized] 
        private ToolsComponentStackEditor _toolsComponentStackEditor;
        public ToolsComponentStackEditor ToolsComponentStackEditor
        {
            get
            {
                if (_toolsComponentStackEditor == null)
                {
                    _toolsComponentStackEditor = new ToolsComponentStackEditor(_toolsComponentStack);
                }

                return _toolsComponentStackEditor;
            }
        }
#endif

        internal void SetupElementStack(Type addGeneralElementsAttribute, Type addElementsAttribute, Type prototypeType)
        {
            _prototypeType = prototypeType;
            _addGeneralElementsAttribute = addGeneralElementsAttribute;
            _addElementsAttribute = addElementsAttribute;
            
            _generalGeneralComponentStack ??= new GeneralComponentStack();
            _toolsComponentStack ??= new ToolsComponentStack();

            _generalGeneralComponentStack.Setup(true, addGeneralElementsAttribute, prototypeType);
            _toolsComponentStack.Setup(true, addElementsAttribute, prototypeType);
        }

        public List<Type> GetAllElementTypes(Type toolType, Type prototypeType = null)
        {
            List<Type> types = GetGeneralElementTypes(toolType, prototypeType);
            types.AddRange(GetElementTypes(toolType, prototypeType));

            return types;
        }

        public List<Type> GetGeneralElementTypes(Type toolType, Type prototypeType = null)
        {
            List<Type> drawTypes = new List<Type>();

            foreach (var attribute in MegaWorldComponentsUtility.GetAttributes(_addGeneralElementsAttribute, _prototypeType, toolType))
            {
                AddComponentsAttribute addComponentsAttribute = (AddComponentsAttribute)attribute;
                
                if (prototypeType != null)
                {
                    if (!addComponentsAttribute.PrototypeTypes.Contains(prototypeType))
                    {
                        continue;
                    }
                }
                
                drawTypes.AddRange(addComponentsAttribute.Types);
            }

            return drawTypes;
        }
	    
        public List<Type> GetElementTypes(Type toolType, Type prototypeType = null)
        {
            List<Type> drawTypes = new List<Type>();

            foreach (var attribute in MegaWorldComponentsUtility.GetAttributes(_addElementsAttribute, _prototypeType, toolType))
            {
                AddComponentsAttribute addComponentsAttribute = (AddComponentsAttribute)attribute;

                if (prototypeType != null)
                {
                    if (!addComponentsAttribute.PrototypeTypes.Contains(prototypeType))
                    {
                        continue;
                    }
                }
                
                drawTypes.AddRange(addComponentsAttribute.Types);
            }

            return drawTypes;
        }
        
        internal void ResetElementsStacks(Type toolType)
        {
            foreach (var type in GetGeneralElementTypes(toolType))
            {
                _generalGeneralComponentStack.Reset(type);
            }

            ToolComponentStack toolComponentStack = ToolsComponentStack.GetToolStackElement(toolType);
            
            toolComponentStack.ComponentStack.Reset();
        }

#if UNITY_EDITOR
        public void DrawToolElements(Type toolType)
        {
            List<Type> drawGeneralTypes = GetGeneralElementTypes(toolType);
            List<Type> drawTypes = GetElementTypes(toolType);

            GeneralComponentStackEditor.DrawElements(drawGeneralTypes);
            ToolsComponentStackEditor.DrawElements(toolType, drawTypes);
        }
        
        public void DrawElement(Type type)
        {
            GeneralComponentStackEditor.DrawElement(type);
        }
        
        public void DrawElement(Type type, Type toolType)
        {
            ToolsComponentStackEditor.DrawElements(toolType, new List<Type> { type });
        }
        
        public void ResetElementsMenu(Type toolType)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Reset"), false, () => ResetElementsStacks(toolType));

            menu.ShowAsContext();
        }
#endif
    }
}