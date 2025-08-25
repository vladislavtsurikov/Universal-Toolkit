using System.Threading;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.ReflectionUtility;
using UnityEngine;

namespace VladislavTsurikov.ActionFlow.Runtime.Actions.GameObject
{
    [Name("GameObject/Set Layer")]
    public class GameObjectLayer : GameObjectAction
    {
        [SerializeField] 
        private LayerMask _layer;
        [SerializeField] 
        private bool _childrenToo = false;
        
        public override string Name => string.Format(
            "Change Layer to {0} on {1} {2}",
            _layer,
            GameObject,
            _childrenToo ? "and children" : string.Empty
        );
        
        protected override UniTask<bool> Run(CancellationToken token)
        {
            if (GameObject == null)
            {
                return UniTask.FromResult(true);
            }

            GameObject.layer = (int)Mathf.Log(_layer.value, 2);

            if (_childrenToo)
            {
                Transform[] children = GameObject.GetComponentsInChildren<Transform>();
                foreach (Transform child in children)
                {
                    child.gameObject.layer = (int)Mathf.Log(_layer.value, 2);
                }
            }

            return UniTask.FromResult(true);
        }
    }
}