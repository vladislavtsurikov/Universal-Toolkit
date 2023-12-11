using System;
using System.Linq;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.Core.Runtime.Interfaces;

namespace VladislavTsurikov.ComponentStack.Runtime
{
    [Serializable]
    public class Element : IHasName
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
                
                return "Missing name";
            }
            set {}
        }
        
        [field: NonSerialized] 
        public bool IsSetup { get; protected set; }
        [field: NonSerialized] 
        public bool IsHappenedReset { get; internal set; }

        protected virtual void SetupElement(object[] args = null){}
        
        protected virtual void OnDisable(){}

        protected virtual void OnReset(Element oldElement){}
        
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
            OnDisable();
            SetupElement(args);
            IsSetup = true;
        }
        
        internal void OnDisableInternal()
        {
            IsSetup = false;
            OnDisable();
        }
        
        internal void OnResetInternal(Element oldElement)
        {
            OnReset(oldElement);
        }
    }
}