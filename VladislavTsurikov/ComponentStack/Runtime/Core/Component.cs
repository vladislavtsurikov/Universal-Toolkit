using OdinSerializer;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;

namespace VladislavTsurikov.ComponentStack.Runtime.Core
{
    public abstract class Component : Element, ISelectable, IRemovable
    {
        [OdinSerialize]
        protected bool _selected;
        
        [OdinSerialize]
        protected bool _active = true;
        
        protected internal object Stack;
        
        public bool Selected
        {
            get => _selected;
            set
            {
                bool changedSelect = _selected != value;

                _selected = value;

                if (changedSelect)
                {
                    if (value)
                    {
                        OnSelect();
                    }
                    else
                    {
                        OnDeselect();
                    }
                }
            }
        }

        public virtual bool Active
        {
            get => _active;
            set
            {
                if (_active != value)
                {
                    _active = value;
                    OnChangeActive();
                }
            }
        }
        
        public bool IsDeletable()
        {
            if (GetType().GetAttribute<PersistentComponentAttribute>() != null)
            {
                return false;
            }
            
            return IsDeletableComponent();
        }

        protected virtual bool IsDeletableComponent()
        {
            return true;
        }

        void IRemovable.OnRemove()
        {
            IsSetup = false;
            OnDeleteElement();
            ((IDisableable)this).OnDisable();
        }

        internal void OnCreateInternal()
        {
            OnCreate();
        }

        protected virtual void OnDeleteElement(){}
        protected virtual void OnCreate(){}
        protected virtual void OnDeselect(){}
        protected virtual void OnSelect(){}
        protected virtual void OnChangeActive(){}
        public virtual bool DeleteElement() { return true; }
    }
}