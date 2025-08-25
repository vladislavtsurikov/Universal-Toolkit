using System.Threading;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.ReflectionUtility;
using UnityEngine;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.ActionFlow.Runtime.Actions.ZenjectIntegration
{
    [Name("GameObject/Instantiate & Registration in DiContainer")]
    public class GameObjectInstantiateWithDiContainer : DiContainerAction
    {
        [SerializeField] private DiContainerMonoBehaviour _prefab;

        protected override string ClassName => nameof(GameObjectInstantiateWithDiContainer);

        protected override UniTask<bool> Run(CancellationToken token)
        {
            if (_prefab == null)
            {
                Debug.LogError($"[{ClassName}] Prefab is not assigned.");
                return UniTask.FromResult(false);
            }
            
            var obj = Object.Instantiate(_prefab);
            obj.SetContainer(DiContainer);
            return UniTask.FromResult(true);
        }
    }
}