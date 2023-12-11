#if UNITY_EDITOR
using VladislavTsurikov.ComponentStack.Editor.Attributes;
using VladislavTsurikov.MegaWorld.Editor.Common;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;

namespace VladislavTsurikov.MegaWorld.Editor.BrushEraseTool.GUI
{
    [ElementEditor(typeof(BrushEraseTool))]
    public class BrushEraseToolEditor : ToolWindowEditor
    {
	    public override void DrawButtons()
		{
			if(WindowData.Instance.SelectedData.HasOneSelectedGroup())
			{
				if (WindowData.Instance.SelectedData.SelectedGroup.PrototypeType == typeof(PrototypeGameObject))
				{
					UndoEditor.OnGUI();
				}
			}
		}
    }
}
#endif