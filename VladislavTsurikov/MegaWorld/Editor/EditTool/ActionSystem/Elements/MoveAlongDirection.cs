#if UNITY_EDITOR
using System.Runtime.Serialization;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Common;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.RendererData;
using VladislavTsurikov.Undo.Editor.Actions.GameObject;
using VladislavTsurikov.Undo.Editor.Actions.TerrainObjectRenderer;

namespace VladislavTsurikov.MegaWorld.Editor.EditTool.ActionSystem.Elements
{
    [MenuItem("Move Up/Down")]
	public class MoveAlongDirection : Action
    {
        private ShortcutCombo.Editor.ShortcutCombo _shortcutCombo;

        private Vector3 _startPosition;
        private float _surfaceOffset;
        
        public MouseSensitivitySettings MouseSensitivitySettings = new MouseSensitivitySettings();
        
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
            _shortcutCombo.AddKey(KeyCode.Q);
        }

        public override void OnMouseMove()
        {
            _surfaceOffset += Event.current.delta.x * MouseSensitivitySettings.MouseSensitivity * 0.05f;

            Vector3 direction = Vector3.up;

            EditTool.FindObject.Position = _startPosition + direction * _surfaceOffset;
        }

        protected override void RegisterUndo()
        {
            if(EditTool.FindObject.PrototypeType == typeof(PrototypeGameObject))
            {
                GameObject go = (GameObject)EditTool.FindObject.Obj;
                Undo.Editor.Undo.RegisterUndoAfterMouseUp(new GameObjectTransform(go));
            }
            else if(EditTool.FindObject.PrototypeType == typeof(PrototypeTerrainObject))
            {
                TerrainObjectInstance instance = (TerrainObjectInstance)EditTool.FindObject.Obj;
                Undo.Editor.Undo.RegisterUndoAfterMouseUp(new TerrainObjectTransform(instance));
            }
        }

        protected override void OnObjectFound()
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
            _startPosition = EditTool.FindObject.Position;
            _surfaceOffset = 0;
        }
    }
}
#endif