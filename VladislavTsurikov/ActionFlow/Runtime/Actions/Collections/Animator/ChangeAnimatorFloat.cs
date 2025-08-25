using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using VladislavTsurikov.DOTweenUtility.Runtime;
using VladislavTsurikov.ReflectionUtility;
using UnityEngine;

namespace VladislavTsurikov.ActionFlow.Runtime.Actions.Animator
{
    [Name("Animator/Change Float")]
    public class ChangeAnimatorFloat : ActionAnimator
    {
        [SerializeField]
        private string _parameter = "My Parameter";
        [SerializeField]
        private float _targetValue = 1f;
        [SerializeField]
        private Transition _transition = new Transition();

        public override string Name => $"Change Animator Float {_parameter}";

        protected override async UniTask<bool> Run(CancellationToken token)
        {
            var tween = DOTween.To(
                () => Animator.GetFloat(_parameter),
                value => Animator.SetFloat(_parameter, value),
                _targetValue,
                _transition.Duration
            ).ApplyTransition(_transition);
            
            token.Register(() => tween.Kill());

            await tween.AsyncWaitForCompletion(_transition).AttachExternalCancellation(token);
            return true;
        }
    }
}