using Cysharp.Threading.Tasks;
using VladislavTsurikov.ComponentStack.Runtime.Core;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.DefaultComponentsSystem
{
    public class DefaultGroupComponent : Component
    {
        protected Group Group;

        protected override UniTask SetupComponent(object[] setupData = null)
        {
            Group = (Group)setupData[0];
            return UniTask.CompletedTask;
        }
    }
}