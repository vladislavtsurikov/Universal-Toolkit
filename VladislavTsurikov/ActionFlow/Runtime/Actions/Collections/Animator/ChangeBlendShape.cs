using System.Threading;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.DOTweenUtility.Runtime;
using VladislavTsurikov.ReflectionUtility;
using UnityEngine;

namespace VladislavTsurikov.ActionFlow.Runtime.Actions.Animator
{
    [Name("Animator/Change Blend Shape")]
    public class ChangeBlendShape : ActionAnimator
    {
        [SerializeField] 
        private SkinnedMeshRenderer _skinnedMesh;
        [SerializeField] 
        private string _blendShape = "Smile";
        [SerializeField] 
        private float _targetValue = 1f;
        [SerializeField] 
        private Transition _transition = new Transition();

        public override string Name => $"Change Blend-Shape {_blendShape}";

        protected override async UniTask<bool> Run(CancellationToken token)
        {
            int blendShapeIndex = _skinnedMesh.sharedMesh.GetBlendShapeIndex(_blendShape);
            if (blendShapeIndex == -1)
            {
                return true;
            }

            var tween = _skinnedMesh
                .DOBlendShapeWeight(blendShapeIndex, _targetValue, _transition.Duration)
                .ApplyTransition(_transition);

            await tween.AsyncWaitForCompletion(_transition);
            return true;
        }
    }
}