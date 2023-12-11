using System;
using UnityEngine;

namespace VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack
{
    /// <summary>
    /// Prevents elements of the same type from being created.
    /// </summary>
    public class ComponentStackOnlyDifferentTypes<T> : AdvancedComponentStack<T>
        where T : Component
    {
        public override void OnRemoveInvalidElements()
        {
            RemoveElementsWithSameType();
        }
        
        public void RemoveElementsWithSameType()
        {
            for (int i = _elementList.Count - 1; i >= 0; i--)
            {
                if (_elementList[i] == null)
                {
                    Debug.Log(_elementList[i] == null);
                }
                if (HasMultipleType(_elementList[i].GetType()))
                {
                    Remove(i);
                }
            }
        }
        
        public void SetupElement<T2>(bool force = false) where T2: Component
        {
            SetupElement(typeof(T2), force);
        }

        public void SetupElement(Type type, bool force = false) 
        {
            if (!typeof(T).IsAssignableFrom(type))
            {
                throw new ArgumentOutOfRangeException(nameof(type));
            }

            Component componentElement = GetElement(type);

            componentElement?.Setup(InitializationDataForElements, force);
        }
        
        public void CreateIfMissingType(Type[] types)
        {
            CreateElementIfMissingType(types);
        }

        public T CreateIfMissingType(Type type)
        {
            return CreateElementIfMissingType(type);
        }
        
        public void AddIfMissingType(T element)
        {
            AddElementIfMissingType(element);
        }

        public bool Remove(Type type)
        {
            GetElement(type, out int index);

            if (index != -1)
            {
                return Remove(index);
            }

            return false;
        }

        public void ReplaceElement(T element)
        {
            for (int i = 0; i < _elementList.Count; i++)
            {
                if (_elementList[i].GetType() == element.GetType())
                {
                    _elementList[i] = element;
                    IsDirty = true;
                    return;
                }
            }

            _elementList.Add(element);
            IsDirty = true;
        }
        
        public void Reset(Type type)
        {
            for (int i = 0; i < _elementList.Count; i++)
            {
                if(_elementList[i].GetType() == type)
                {
                    Reset(i);
                }
            }
        }
        
        protected bool HasMultipleType(Type type)
        {
            int count = 0;
            
            foreach (var element in _elementList)
            {
                if (element != null)
                {
                    if (element.GetType().ToString() == type.ToString())
                    {
                        count++;
                    }

                    if (count == 2)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool HasType(Type type)
        {
            return GetElement(type) != null;
        }
    }
}