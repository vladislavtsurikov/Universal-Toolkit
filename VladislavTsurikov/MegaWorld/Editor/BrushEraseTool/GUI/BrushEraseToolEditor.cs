#if UNITY_EDITOR
using VladislavTsurikov.ComponentStack.Editor.Attributes;
using VladislavTsurikov.MegaWorld.Editor.Common;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;

namespace VladislavTsurikov.MegaWorld.Editor.BrushEraseTool.GUI
{
    [ElementEditor(typeof(BrushEraseTool))]
    public class BrushEraseToolEditor : ToolWindowEditor
    {
	    public override void DrawButtons()
		{
			UndoEditor.DrawButtons(TargetType, WindowData.Instance.SelectedData);
		}
    }
}
#endif