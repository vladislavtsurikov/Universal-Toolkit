using System.Threading;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.ReflectionUtility;
using UnityEngine;

namespace VladislavTsurikov.ActionFlow.Runtime.Actions.Log
{
    [Name("Debug/Pause Editor")]
    public class PauseEditor : Action
    {
        public override string Name => "Pause Editor";

        protected override UniTask<bool> Run(CancellationToken token)
        {
#if UNITY_EDITOR
            Debug.Break();
#endif
            return UniTask.FromResult(true);
        }
    }
}