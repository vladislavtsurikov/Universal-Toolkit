using System;
using System.Collections.Generic;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.ReflectionUtility.Runtime;
using VladislavTsurikov.Utility.Runtime;

namespace VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack
{
    public abstract class AdvancedComponentStack<T> : ComponentStack<T>
        where T : Component
    {
        /// <summary>
        /// A public link to the list is required for UnityEditorInternal.ReorderableList
        /// </summary>
        public AdvancedElementList<T> ReorderableElementList => _elementList;
        
        private protected override void CreateElements()
        {
            OnCreateElements();
            
            CreateComponentsAttribute componentsAttribute = GetType().GetAttribute<CreateComponentsAttribute>();

            if (componentsAttribute != null)
            {
                CreateElementIfMissingType(componentsAttribute.Types.ToArray());
            }
        }

        protected virtual void OnCreateElements()
        {
        }

        public void CreateAllElementTypes()
        {
            CreateElementIfMissingType(AllTypesDerivedFrom<T>.TypeList.ToArray());
        }
        
        protected void CreateElementIfMissingType(Type[] types)
        {
            foreach (Type type in types)
            {
                CreateElementIfMissingType(type);
            }
        }

        protected T CreateElementIfMissingType(Type type)
        {
            T settings = GetElement(type);
            if (settings == null)
            {
                return Create(type);
            }

            return settings;
        }
        
        protected void AddElementIfMissingType(T element)
        {
            T settings = GetElement(element.GetType());
            
            if (settings == null)
            {
                Add(element);
            }
        }
        
        public T GetElement(Type type)
        {
            return GetElement(type, out _);
        }
        
        public T GetElement(Type type, out int index)
        {
            index = -1;
            for (int i = 0; i < _elementList.Count; i++)
            {
                if (_elementList[i] != null)
                {
                    if (_elementList[i].GetType() == type)
                    {
                        index = i;
                        return _elementList[i];
                    }
                }
            }

            return null;
        }
    }
}