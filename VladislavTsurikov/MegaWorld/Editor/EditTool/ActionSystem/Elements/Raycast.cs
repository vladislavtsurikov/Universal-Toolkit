#if UNITY_EDITOR
using System.Runtime.Serialization;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime.Scene;
using VladislavTsurikov.ColliderSystem.Runtime.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.RendererData;
using VladislavTsurikov.Undo.Editor.Actions.GameObject;
using VladislavTsurikov.Undo.Editor.Actions.TerrainObjectRenderer;

namespace VladislavTsurikov.MegaWorld.Editor.EditTool.ActionSystem.Elements
{
    [ComponentStack.Runtime.Attributes.MenuItem("Raycast")]
	public class Raycast : Action
    {
        private ShortcutCombo.Editor.ShortcutCombo _shortcutCombo;

        public float RaycastPositionOffset = 0;
        public LayerMask GroundLayers = 1;
        
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
            _shortcutCombo.AddKey(KeyCode.W);
        }

        public override void OnMouseMove()
        {
            RayHit rayHit = ColliderUtility.Raycast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), GroundLayers);
            if(rayHit != null)
            {
                Vector3 finalPosition = new Vector3(rayHit.Point.x, rayHit.Point.y + RaycastPositionOffset, rayHit.Point.z);
                EditTool.FindObject.Position = finalPosition;
            }
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
