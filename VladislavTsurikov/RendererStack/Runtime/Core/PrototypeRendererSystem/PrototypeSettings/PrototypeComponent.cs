using VladislavTsurikov.ComponentStack.Runtime.Core;

namespace VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.PrototypeSettings
{
    public abstract class PrototypeComponent : Component
    {
        public Prototype Prototype { get; private set; }

        protected override void SetupElement(object[] args = null)
        {
            Prototype = (Prototype)args[0];
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
