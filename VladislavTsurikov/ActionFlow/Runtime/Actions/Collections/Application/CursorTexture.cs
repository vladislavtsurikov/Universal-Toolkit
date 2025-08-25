using System.Threading;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.ReflectionUtility;
using UnityEngine;

namespace VladislavTsurikov.ActionFlow.Runtime.Actions.Application
{
    [Name("Application/Cursor Texture")]
    public class CursorTexture : Action
    {
        [SerializeField]
        private Texture2D _texture;
        [SerializeField]
        private Vector2 _tip = Vector2.zero;
        [SerializeField]
        private CursorMode _mode = CursorMode.Auto;

        public override string Name => $"Set Cursor Texture to {_texture}";

        protected override UniTask<bool> Run(CancellationToken token)
        {
            Cursor.SetCursor(_texture, _tip, _mode);
            return UniTask.FromResult(true);
        }
    }
}