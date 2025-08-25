using System.Threading;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.ReflectionUtility;
using OdinSerializer;

namespace VladislavTsurikov.ActionFlow.Runtime.Actions.Utility
{
    [Name("Utility/Action Stack")]
    public class ActionStack : Action
    {
        [OdinSerialize]
        public ActionCollection ActionCollection = new ActionCollection();

        protected override async UniTask<bool> Run(CancellationToken token)
        {
            await ActionCollection.Run(token);
            return true;
        }
    }
}