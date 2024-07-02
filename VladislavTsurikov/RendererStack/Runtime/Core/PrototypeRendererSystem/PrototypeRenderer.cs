using VladislavTsurikov.OdinSerializer.Core.Misc;
using VladislavTsurikov.RendererStack.Runtime.Core.RendererSystem;

namespace VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem
{
    public abstract class PrototypeRenderer : Renderer
    {
        [OdinSerialize]
        public SelectionData SelectionData = new SelectionData();

        protected override void SetupRenderer()
        {
            SelectionData.Setup(this);

            SetupPrototypeRendererRenderer();
        }

        public override void CheckChanges()
        {
            SelectionData.CheckPrototypeChanges(); 
        }

        protected virtual void SetupPrototypeRendererRenderer(){}
    }
}