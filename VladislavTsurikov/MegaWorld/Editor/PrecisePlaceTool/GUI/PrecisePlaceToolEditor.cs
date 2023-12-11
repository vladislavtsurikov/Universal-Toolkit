#if UNITY_EDITOR
using VladislavTsurikov.ComponentStack.Editor.Attributes;
using VladislavTsurikov.MegaWorld.Editor.Common;
using VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;
using VladislavTsurikov.MegaWorld.Editor.PrecisePlaceTool.PrototypeSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.OverlapCheckSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;

namespace VladislavTsurikov.MegaWorld.Editor.PrecisePlaceTool.GUI
{
    [ElementEditor(typeof(PrecisePlaceTool))]
	[DrawersSelectionDatas(typeof(PrecisePlacePrototypesDrawer))]
    public class PrecisePlaceToolEditor : ToolWindowEditor
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
		}

	    protected override void DrawPrototypeSettings(Prototype proto)
		{
			PrecisePlaceToolSettings settings = (PrecisePlaceToolSettings)ToolsComponentStack.GetElement(typeof(PrecisePlaceTool), typeof(PrecisePlaceToolSettings));
			
			proto.ComponentStackManager.DrawElement(typeof(PrecisePlaceSettings), typeof(PrecisePlaceTool));
			proto.ComponentStackManager.DrawElement(typeof(SuccessSettings));

			if(settings.OverlapCheck)
			{
				proto.ComponentStackManager.DrawElement(typeof(OverlapCheckSettings));
			}

			if(settings.UseTransformComponents)
			{
				proto.ComponentStackManager.DrawElement(typeof(TransformComponentSettings));
			}
		}
    }
}
#endif