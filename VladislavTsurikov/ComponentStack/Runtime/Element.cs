using System;
using System.Linq;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.ComponentStack.Runtime.Interfaces;
using VladislavTsurikov.Core.Runtime.Interfaces;

namespace VladislavTsurikov.ComponentStack.Runtime
{
    [Serializable]
    public class Element : IHasName, ISetup, IDisable
    {
        [NonSerialized] 
        public string RenamingName;
        [NonSerialized] 
        public bool Renaming;
        
        public bool SelectSettingsFoldout = true;

        public virtual string Name
        {
            get
            {
                MenuItemAttribute menuItemAttribute = GetType().GetAttribute<MenuItemAttribute>();
                
                if (menuItemAttribute != null)
                {
                    return menuItemAttribute.Name.Split('/').Last(); 
                }
                else
                {
                    return GetType().ToString().Split('.').Last();
                }
            }
            set {}
        }
        
        [field: NonSerialized] 
        public bool IsSetup { get; protected set; }
        [field: NonSerialized] 
        public bool IsHappenedReset { get; internal set; }

        protected virtual void SetupElement(object[] args = null){}

        protected virtual void OnDisableElement(){}

        protected virtual void OnResetElement(Element oldElement){}
        
        public virtual bool ShowActiveToggle()
        {
            return true;
        }
        
        public void Setup(object[] args = null, bool force = false)
        {
            if (!force)
            {
                if (IsSetup)
                {
                    return;
                }
            }
            
            IsSetup = false;
            OnDisableElement();
            SetupElement(args);
            IsSetup = true;
        }

        void IDisable.OnDisable()
        {
            IsSetup = false;
            OnDisableElement();
        }
        
        internal void OnReset(Element oldElement)
        {
            OnResetElement(oldElement);
        }
    }
}