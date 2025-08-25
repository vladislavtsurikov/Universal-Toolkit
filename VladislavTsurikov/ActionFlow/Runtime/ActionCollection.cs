using System.Threading;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ActionFlow.Runtime.Actions;

namespace VladislavTsurikov.ActionFlow.Runtime
{
    public class ActionCollection : ComponentStackSupportSameType<Action>
    {
        public async UniTask<bool> Run(CancellationToken token = default)
        {
            foreach (var action in ElementList)
            {
                token.ThrowIfCancellationRequested();
                bool isActionCompleted =  await action.RunAction(token);

                if (!isActionCompleted)
                {
                    return false;
                }
            }

            return true;
        }
    }
}