#if UNITY_EDITOR
using System.Runtime.Serialization;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.Undo.Editor.UndoActions;

namespace VladislavTsurikov.MegaWorld.Editor.EditTool.ActionSystem.Elements
{
    [MenuItem("Remove")]
	public class Remove : Action
    {
        private ShortcutCombo.Editor.ShortcutCombo _shortcutCombo;

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
            _shortcutCombo.AddKey(KeyCode.T);
        }

        public override void ObjectFound()
        {            
            EditTool.FindObject.DestroyObject();
            EditTool.FindObject = null;
        }

        public override void UndoCall()
        {
            if(EditTool.FindObject.PrototypeType == typeof(PrototypeGameObject))
            {
                GameObject go = (GameObject)EditTool.FindObject.Obj;
                Undo.Editor.Undo.RegisterUndoAfterMouseUp(new DestroyedGameObject(go));
            }
        }

        public override Color GetColorHandleButton(){return new Color(1f, 0f, 0f, 0.7f);}

        public override bool CheckShortcutCombo()
        {
            if(_shortcutCombo.IsActive())
            {
                return true;
            }

            return false;
        }
	}
}
#endif