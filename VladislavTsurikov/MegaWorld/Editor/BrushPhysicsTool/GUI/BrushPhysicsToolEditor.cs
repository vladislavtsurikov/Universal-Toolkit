#if UNITY_EDITOR
using System;
using VladislavTsurikov.ComponentStack.Editor.Attributes;
using VladislavTsurikov.MegaWorld.Editor.Common;
using VladislavTsurikov.MegaWorld.Editor.Common.Settings.PhysicsToolsSettings;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.PhysicsSimulatorEditor.Editor;

namespace VladislavTsurikov.MegaWorld.Editor.BrushPhysicsTool.GUI 
{
	[Serializable]
	[ElementEditor(typeof(BrushPhysicsTool))]
	public class BrushPhysicsToolEditor : ToolWindowEditor
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
		
		public override void DrawFirstSettings()
		{
			PhysicsSimulatorSettingsEditor.OnGUI(PhysicsSimulatorSettings.Instance, DisablePhysicsMode.ObjectTime);
		}
	}
}
#endif