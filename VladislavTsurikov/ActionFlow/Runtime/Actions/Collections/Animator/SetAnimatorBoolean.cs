using System.Threading;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.ReflectionUtility;
using UnityEngine;

namespace VladislavTsurikov.ActionFlow.Runtime.Actions.Animator
{
    [Name("Animator/Set Boolean")]
    public class SetAnimatorBoolean : ActionAnimator
    {
        [SerializeField]
        private string _parameter = "My Parameter";
        [SerializeField]
        private bool _value = true;

        public override string Name => $"Set Animator Boolean {_parameter}";

        protected override UniTask<bool> Run(CancellationToken token)
        {
            Animator.SetBool(_parameter, _value);
            return UniTask.FromResult(true);
        }
    }
}