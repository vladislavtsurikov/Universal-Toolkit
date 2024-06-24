using System;
using VladislavTsurikov.OdinSerializer.Core.Misc;
using VladislavTsurikov.OdinSerializer.Unity_Integration.SerializedUnityObjects;
using Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.MonoBehaviour
{
    public abstract class MonoBehaviourTool : SerializedMonoBehaviour
    {
        [OdinSerialize]
        private ComponentStack _componentStack = new ComponentStack();

        [field: NonSerialized]
        public bool IsSetup { get; private set; }

        internal ComponentStack ComponentStack => _componentStack;

        public SelectionDatas.SelectionData Data = new SelectionDatas.SelectionData();

        private void OnEnable()
        {
            Setup();
        }

        private void Update()
        {
            Data.DeleteNullElements();
            Data.SelectedData.SetSelectedData();

            OnUpdate();
        }
        
        public Component GetElement(Type elementType)
        {
            return _componentStack.GetElement(elementType);
        }

        public void Setup()
        {
            Data.Setup();
            _componentStack.Setup(true, this);
            OnToolEnable();

            IsSetup = true; 
        }

        protected virtual void OnUpdate()
        {
        }
        
        private protected virtual void OnToolEnable()
        {
        }
    }
}