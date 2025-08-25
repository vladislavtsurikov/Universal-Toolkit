using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.ComponentStack.Runtime.Core
{
    [Serializable]
    public class Element : IHasName, IDisableable
    {
        public bool SelectSettingsFoldout = true;
        private object[] _setupData;

        [NonSerialized]
        public bool Renaming;

        [NonSerialized]
        public string RenamingName;

        [field: NonSerialized]
        public bool IsSetup { get; protected set; }

        [field: NonSerialized]
        public bool IsHappenedReset { get; internal set; }

        void IDisableable.OnDisable()
        {
            IsSetup = false;
            OnDisableElement();
        }

        public virtual string Name
        {
            get
            {
                NameAttribute nameAttribute = GetType().GetAttribute<NameAttribute>();

                if (nameAttribute != null)
                {
                    return nameAttribute.Name.Split('/').Last();
                }

                return GetType().ToString().Split('.').Last();
            }
            set { }
        }

        protected virtual UniTask SetupComponent(object[] setupData = null) => UniTask.CompletedTask;

        protected virtual UniTask FirstSetupComponent(object[] setupData = null) => UniTask.CompletedTask;

        protected virtual void OnDisableElement()
        {
        }

        protected virtual void OnResetElement(Element oldElement)
        {
        }

        public virtual bool ShowActiveToggle() => true;

        public async UniTask Setup(bool force = false) => await SetupWithSetupData(force, _setupData);

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

        internal void OnReset(Element oldElement) => OnResetElement(oldElement);
    }
}
