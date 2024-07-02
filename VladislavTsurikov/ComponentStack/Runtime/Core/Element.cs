using System;
using System.Linq;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;

namespace VladislavTsurikov.ComponentStack.Runtime.Core
{
    [Serializable]
    public class Element : IHasName, ISetupable, IDisableable
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
                NameAttribute nameAttribute = GetType().GetAttribute<NameAttribute>();
                
                if (nameAttribute != null)
                {
                    return nameAttribute.Name.Split('/').Last(); 
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

        protected virtual void SetupComponent(object[] setupData = null){}

        protected virtual void OnDisableElement(){}

        protected virtual void OnResetElement(Element oldElement){}
        
        public virtual bool ShowActiveToggle()
        {
            return true;
        }
        
        public void Setup(object[] setupData = null, bool force = false)
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
            SetupComponent(setupData);
            IsSetup = true;
        }

        void IDisableable.OnDisable()
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