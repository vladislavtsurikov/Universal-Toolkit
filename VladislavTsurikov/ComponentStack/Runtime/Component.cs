using VladislavTsurikov.Core.Runtime.Interfaces;
using VladislavTsurikov.OdinSerializer.Core.Misc;

namespace VladislavTsurikov.ComponentStack.Runtime
{
    public abstract class Component : Element, ISelected
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

        public virtual bool IsDeletable()
        {
            return true;
        }

        internal void OnDeleteInternal()
        {
            IsSetup = false;
            OnDelete();
            OnDisable();
        }

        internal void OnCreateInternal()
        {
            OnCreate();
        }

        protected virtual void OnDelete(){}
        protected virtual void OnCreate(){}
        protected virtual void OnDeselect(){}
        protected virtual void OnSelect(){}
        protected virtual void OnChangeActive(){}
        public virtual bool IsValid() { return true; }
    }
}