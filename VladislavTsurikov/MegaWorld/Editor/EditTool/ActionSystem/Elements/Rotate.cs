#if UNITY_EDITOR
using System.Runtime.Serialization;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Common;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.Undo.Editor.UndoActions;
using VladislavTsurikov.Utility.Runtime;

namespace VladislavTsurikov.MegaWorld.Editor.EditTool.ActionSystem.Elements
{
    [MenuItem("Rotate")]
	public class Rotate : Action
    {
        private ShortcutCombo.Editor.ShortcutCombo _shortcutCombo;

        private float _rotationDist;
        private Quaternion _startRotation;
        private Quaternion _сurrentRotation;
        private Vector3 _rotationAxis;

        public MouseSensitivitySettings MouseSensitivitySettings = new MouseSensitivitySettings();
        public bool EnableSnapRotate = false;
        public float SnapRotate = 15f;

        [OnDeserializing]
        private void OnDeserializing()
        {
            InitShortcutCombo();
        }

        protected override void OnCreate()
        {
            InitShortcutCombo();
        }
        
        private void InitShortcutCombo()
        {
            _shortcutCombo = new ShortcutCombo.Editor.ShortcutCombo();
            _shortcutCombo.AddKey(KeyCode.E);
        }

        public override void OnMouseMove()
        {
            _rotationDist += Event.current.delta.x * MouseSensitivitySettings.MouseSensitivity * 2f;
            float angle = _rotationDist;
            if(EnableSnapRotate)
            {
                angle = Snapping.Snap(_rotationDist, SnapRotate);
            }
            
            _сurrentRotation = Quaternion.AngleAxis(angle, _rotationAxis);
            EditTool.FindObject.Rotation = _сurrentRotation * _startRotation;
        }

        public override void UndoCall()
        {
            if(EditTool.FindObject.PrototypeType == typeof(PrototypeGameObject))
            {
                GameObject go = (GameObject)EditTool.FindObject.Obj;
                Undo.Editor.Undo.RegisterUndoAfterMouseUp(new GameObjectTransform(go));
            }
        }

        public override void ObjectFound()
        {
            SetStartValue();
        }
        
        public override bool CheckShortcutCombo()
        {
            if(_shortcutCombo.IsActive())
            {
                return true;
            }

            return false;
        }

        private void SetStartValue()
        {
            _rotationAxis = Vector3.up;
            if(GlobalCommonComponentSingleton<TransformSpaceSettings>.Instance.TransformSpace == TransformSpace.Local)
            {
                _rotationAxis = EditTool.FindObject.Rotation * _rotationAxis;
            }

            _rotationDist = 0;
            _startRotation = EditTool.FindObject.Rotation;
            _сurrentRotation = Quaternion.identity;
        }
    }
}
#endif