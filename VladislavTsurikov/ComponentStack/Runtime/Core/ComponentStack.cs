using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using OdinSerializer;
using OdinSerializer.Utilities;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;

namespace VladislavTsurikov.ComponentStack.Runtime.Core
{
    [Serializable]
    public abstract class ComponentStack<T> where T : Component
    {
        [NonSerialized] 
        public bool IsDirty = true;
        
        [OdinSerialize]
        protected AdvancedElementList<T> _elementList = new AdvancedElementList<T>();

        public object[] SetupData { get; private set; }
        
        public IReadOnlyList<T> ElementList => _elementList;

        public bool IsSetup { get; private set; }
        
        public event Action<int> ElementAdded;
        public event Action<int> ElementRemoved;
        public event Action ListChanged;

        public T SelectedElement
        {
            get { return _elementList.FirstOrDefault(t => t.Selected); }
        }
        
        public async UniTask Setup(bool force = true, object[] setupData = null, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _elementList ??= new AdvancedElementList<T>();

            _elementList.OnAdded += HandleElementAdded;
            _elementList.OnRemoved += HandleElementRemoved;
            _elementList.OnListChanged += HandleListChanged;

            IsSetup = true;

            SetupData = setupData;

            OnSetup();
            RemoveInvalidElements();
            CreateElements();

            foreach (var element in _elementList)
            {
                cancellationToken.ThrowIfCancellationRequested();
                element.Stack = this;
                await element.SetupWithSetupData(force, setupData);
            }

            IsDirty = true;
        }

        public void OnDisable()
        {
            IsSetup = false;

            for (int i = 0; i < _elementList.Count; i++)
            {
                ((IDisableable)_elementList[i]).OnDisable();
            }
            
            _elementList.OnAdded -= HandleElementAdded;
            _elementList.OnRemoved -= HandleElementRemoved;
            _elementList.OnListChanged -= HandleListChanged;

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
            element.SetupWithSetupData(true, SetupData);
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
                    element.SetupWithSetupData(true, SetupData);
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
                _elementList.Insert(index, element);
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

        public void Select(T element)
        {
            _elementList.ForEach(setting => setting.Selected = false);
            element.Selected = true;
        }

        public void RemoveInvalidElements()
        {
            for (int i = _elementList.Count - 1; i >= 0; i--)
            {
                if (_elementList[i] == null || _elementList[i].GetType().IsAbstract || !_elementList[i].DeleteElement())
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
        
        private void HandleElementAdded(int index)
        {
            ElementAdded?.Invoke(index);
        }

        private void HandleElementRemoved(int index)
        {
            ElementRemoved?.Invoke(index);
        }

        private void HandleListChanged()
        {
            ListChanged?.Invoke();
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