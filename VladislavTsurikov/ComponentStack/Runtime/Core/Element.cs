using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.ComponentStack.Runtime.Core
{
    [Serializable]
    public class Element : IHasName, IDisableable
    {
        private object[] _setupData;
        
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

        protected virtual UniTask SetupComponent(object[] setupData = null)
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask FirstSetupComponent(object[] setupData = null)
        {
            return UniTask.CompletedTask;
        }

        protected virtual void OnDisableElement(){}

        protected virtual void OnResetElement(Element oldElement){}
        
        public virtual bool ShowActiveToggle()
        {
            return true;
        }
        
        public async UniTask Setup(bool force = false)
        {
            await SetupWithSetupData(force, _setupData);
        }
        
        public async UniTask SetupWithSetupData(bool force = false, object[] setupData = null)
        {
            if (!force && IsSetup)
            {
                return;
            }
            
            _setupData = setupData;

            IsSetup = false;
            OnDisableElement();
            
            if (!IsSetup)
            {
                FirstSetupComponent(setupData);
            }
            
            await SetupComponent(setupData);
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