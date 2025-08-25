using System;
using Cysharp.Threading.Tasks;
using OdinSerializer;
using Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;
using Core_Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;
using Runtime_Core_Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

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
        
        public Runtime_Core_Component GetElement(Type elementType)
        {
            return _componentStack.GetElement(elementType);
        }

        public void Setup()
        {
            Data.Setup();
            _componentStack.Setup(true, new object[]{this}).Forget();
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