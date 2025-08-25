using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using VladislavTsurikov.DOTweenUtility.Runtime;
using VladislavTsurikov.ReflectionUtility;
using UnityEngine;

namespace VladislavTsurikov.ActionFlow.Runtime.Actions.Animator
{
    [Name("Animator/Change Integer")]
    public class ChangeAnimatorInteger : ActionAnimator
    {
        [SerializeField]
        private string _parameter = "My Parameter";
        [SerializeField]
        private int _targetValue = 1;
        [SerializeField]
        private Transition _transition = new Transition();

        public override string Name => $"Change Animator Integer {_parameter}";

        protected override async UniTask<bool> Run(CancellationToken token)
        {
            var tween = DOTween.To(
                () => Animator.GetInteger(_parameter),
                value => Animator.SetInteger(_parameter, value),
                _targetValue,
                _transition.Duration
            ).ApplyTransition(_transition);

            await tween.AsyncWaitForCompletion(_transition);
            return true;
        }
    }
}