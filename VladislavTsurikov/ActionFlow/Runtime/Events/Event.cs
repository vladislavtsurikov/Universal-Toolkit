using Cysharp.Threading.Tasks;
using Core_Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

namespace VladislavTsurikov.ActionFlow.Runtime.Events
{
    public abstract class Event : Core_Component
    {
        internal Trigger Trigger;
        
        protected override UniTask SetupComponent(object[] setupData = null)
        {
            Trigger = (Trigger)setupData[0];
            return UniTask.CompletedTask;
        }
    }
}