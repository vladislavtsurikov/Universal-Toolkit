using System;
using System.Collections.Generic;
using System.Linq;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.OdinSerializer.Core.Misc;
using VladislavTsurikov.OdinSerializer.Utilities;

namespace VladislavTsurikov.ComponentStack.Runtime.Core
{
    [Serializable]
    public abstract class ComponentStack<T> where T : Component
    {
        [OdinSerialize]
        protected AdvancedElementList<T> _elementList = new AdvancedElementList<T>();

        public object[] SetupData { get; private set; }
        
        public IReadOnlyList<T> ElementList => _elementList;

        [NonSerialized] 
        public bool IsDirty = true;
        public bool IsSetup { get; private set; }

        public T SelectedElement
        {
            get { return _elementList.FirstOrDefault(t => t.Selected); }
        }

        public void Setup(bool force = true, params object[] setupData)
        {
            _elementList ??= new AdvancedElementList<T>();
            
            IsSetup = true;

            IsDirty = true;
            SetupData = setupData;

            OnSetup();
            RemoveInvalidElements();
            CreateElements();

            for (int i = 0; i < _elementList.Count; i++)
            {
                _elementList[i].Stack = this;
                _elementList[i].Setup(setupData, force);
            }
        }

        public void OnDisable()
        {
            IsSetup = false;

            for (int i = 0; i < _elementList.Count; i++)
            {
                ((IDisableable)_elementList[i]).OnDisable();
            }

            OnDisableStack();
        }

        public void Clear()
        {
            for (int i = _elementList.Count - 1; i >= 0; i--)
            {
                if (Remove(i))
                {
                    IsDirty = true;
                }
            }
        }

        protected T Create(Type type, int index = -1)
        {
            if (!AllowCreate(type))
            {
                return null;
            }
            
            if (type.GetAttribute(typeof(DontCreateAttribute)) != null)
            {
                return null;
            }
            
            var element = Instantiate(type, false);
            Add(element, index);
            element.Stack = this;
            element.Setup(SetupData);
            element.OnCreateInternal();

            return element;
        }

        public T Instantiate(Type type, bool setup = true)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (typeof(T).IsAssignableFrom(type))
            {
                var element = (T)Activator.CreateInstance(type);

                if (setup)
                {
                    element.Setup(SetupData);
                    element.OnCreateInternal();
                }

                return element;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(type));
            }
        }

        protected void Add(T element, int index = -1)
        {
            if (element == null)
            {
                return;
            }
            
            if (!AllowCreate(element.GetType()))
            {
                return;
            }
            
            if (index == -1)
            {
                _elementList.Add(element);
            }
            else
            {
                _elementList[index] = element;
            }

            if (_elementList.Count == 1)
            {
                element.Selected = true;
            }

            IsDirty = true;
        }
        
        public void Reset()
        {
            for (int i = 0; i < ElementList.Count; i++)
            {
                Reset(i);
            }
        }
        
        public void Reset(int index)
        {
            T oldElement = _elementList[index];
            oldElement.IsHappenedReset = true;
            
            T newElement = Create(oldElement.GetType(), index);
            newElement.Stack = this;
            
            newElement.OnReset(oldElement);
            
            IsDirty = true;
        }

        public void RemoveAll()
        {
            for (int i = _elementList.Count - 1; i >= 0; i--)
            {
                Remove(i);
            }
        }
        
        public bool Remove(int index)
        {
            var element = _elementList[index];
            
            if (element == null || element.IsDeletable())
            {
                _elementList.RemoveAt(index);
                IsDirty = true;
                return true;
            }
            
            return false;
        }

        public void ReplaceElement(T element, int index)
        {
            if (!Remove(index))
            {
                return;
            }

            _elementList.Insert(index, element);
            IsDirty = true;
        }

        public void Select(T stackElement)
        {
            _elementList.ForEach(setting => setting.Selected = false);
            stackElement.Selected = true;
        }

        public void RemoveInvalidElements()
        {
            for (int i = _elementList.Count - 1; i >= 0; i--)
            {
                if (_elementList[i] == null || _elementList[i].GetType().IsAbstract || !_elementList[i].IsValid())
                {
                    Remove(i);
                }
            }
            
            OnRemoveInvalidElements();
        }
        
        public List<T> FindAll(Predicate<T> match)
        {
            if (match == null)
            {
                return new List<T>();
            }
                
            List<T> all = new List<T>();
            
            foreach (var element in _elementList)
            {
                if (match(element))
                {
                    all.Add(element);
                }
            }
            
            return all;
        }

        protected virtual void OnSetup()
        {
        }

        protected virtual bool AllowCreate(Type type)
        {
            return true;
        }

        private protected virtual void CreateElements()
        {
        }

        protected virtual void OnDisableStack()
        {
        }

        public virtual void OnRemoveInvalidElements()
        {
        }
    }
}