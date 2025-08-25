using Cysharp.Threading.Tasks;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using Zenject;

namespace VladislavTsurikov.ComponentStack.Runtime
{
    public abstract class DiContainerComponent : Component
    {
        protected DiContainer DiContainer;
        
        protected override async UniTask SetupComponent(object[] setupData = null)
        {
            if (setupData == null)
            {
                return;
            }
            
            DiContainer = (DiContainer)setupData[0];
            InjectSelf();
            await SetupDiContainerComponent(setupData);
        }

        protected virtual UniTask SetupDiContainerComponent(object[] setupData = null)
        {
            return UniTask.CompletedTask;
        }

        protected void InjectSelf()
        {
            DiContainer.Inject(this);
        }
    }
}