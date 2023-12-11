#if UNITY_EDITOR
using VladislavTsurikov.ComponentStack.Editor.Attributes;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList.Attributes;
using VladislavTsurikov.MegaWorld.Editor.Common;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;
using VladislavTsurikov.MegaWorld.Editor.Core.Window.ElementSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;

namespace VladislavTsurikov.MegaWorld.Editor.EditTool
{
	[ElementEditor(typeof(EditTool))]
    [DontDrawFoldout]
    public class EditToolEditor : ToolWindowEditor
    {
	    public override void DrawButtons()
		{
			if(WindowData.Instance.SelectedData.HasOneSelectedGroup())
			{
				if(WindowData.Instance.SelectedData.SelectedGroup.PrototypeType == typeof(PrototypeGameObject))
				{
					UndoEditor.OnGUI();
				}
			}
			
			EditToolSettingsEditor editor = (EditToolSettingsEditor)ToolsComponentStackEditor.GetEditor(typeof(EditTool), typeof(EditToolSettings));
			editor.ActionStackEditor.DrawButtons();
		}

	    protected override void DrawCommonSettings() { }
    }
}
#endif