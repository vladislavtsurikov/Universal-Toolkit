#if UNITY_EDITOR
using System;
using Cysharp.Threading.Tasks;
using OdinSerializer;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.MegaWorld.Editor.EditTool.ActionSystem;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Editor.EditTool
{
    [Serializable]
    [Name("Edit Tool Settings")]
    public class EditToolSettings : Component
    {
        public float SphereSize = 40;
        public float MaxDistance = 200;

        [OdinSerialize]
        private ActionStack _actionStack = new();

        public ActionStack ActionStack
        {
            get
            {
                _actionStack.RemoveInvalidElements();
                return _actionStack;
            }
        }

        protected override UniTask SetupComponent(object[] setupData = null) => _actionStack.Setup();
    }
}
#endif
