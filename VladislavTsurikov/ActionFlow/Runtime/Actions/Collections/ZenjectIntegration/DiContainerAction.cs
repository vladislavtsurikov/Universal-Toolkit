#if ACTIONFLOW_ZENJECT
using Cysharp.Threading.Tasks;
using Zenject;

namespace VladislavTsurikov.ActionFlow.Runtime.Actions.ZenjectIntegration
{
    public abstract class DiContainerAction : Action
    {
        protected abstract string ClassName { get; }

        protected DiContainer DiContainer { get; private set; }
        protected bool IsErrorInDiContainer { get; private set; }
        protected string ErrorMessage { get; private set; }

        protected override UniTask SetupComponent(object[] setupData = null)
        {
            if (setupData == null)
            {
                return UniTask.CompletedTask;
            }

            DiContainer = (DiContainer)setupData[0];
            SetupDiContainerAction();
            return UniTask.CompletedTask;
        }

        protected virtual void SetupDiContainerAction()
        {
        }

        protected T TryResolve<T>() where T : class
        {
            var component = DiContainer.TryResolve(typeof(T));
            if (component == null)
            {
                IsErrorInDiContainer = true;
                ErrorMessage = $"[{ClassName}] {nameof(T)} is not find.";
                return null;
            }

            return component as T;
        }

        protected void SetError(string problemClass)
        {
            IsErrorInDiContainer = true;
            ErrorMessage = $"[{ClassName}] {problemClass} is not find.";
        }
    }
}
#endif
