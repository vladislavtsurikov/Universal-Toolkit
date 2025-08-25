using System.Threading;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.DOTweenUtility.Runtime;
using VladislavTsurikov.ReflectionUtility;
using UnityEngine;

namespace VladislavTsurikov.ActionFlow.Runtime.Actions.Animator
{
    [Name("Animator/Change Layer")]
    public class ChangeAnimatorLayer : ActionAnimator
    {
        [SerializeField]
        private int _layerIndex = 1;
        [SerializeField]
        private float _targetWeight = 1f;
        [SerializeField]
        private Transition _transition = new Transition();

        public override string Name => $"Change Layer {_layerIndex} Weight";

        protected override async UniTask<bool> Run(CancellationToken token)
        {
            var tween = Animator
                .DOSetLayerWeight(_layerIndex, _targetWeight, _transition.Duration)
                .ApplyTransition(_transition);

            await tween.AsyncWaitForCompletion(_transition);
            return true;
        }
    }
}