using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.SelectionDatas;

namespace VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.PrototypeSettings
{
    public abstract class PrototypeComponent : ComponentStack.Runtime.Component
    {
        private Prototype _prototype;

        public Prototype Prototype => _prototype;

        protected override void SetupElement(object[] args = null)
        {
            _prototype = (Prototype)args[0];
            SetupPrototypeComponent();
        }

        protected virtual void SetupPrototypeComponent(){}

        public static bool IsValid(PrototypeComponent component)
        {
            if (component == null || !component.Active)
            {
                return false;
            }

            return true;
        }
    }
}
