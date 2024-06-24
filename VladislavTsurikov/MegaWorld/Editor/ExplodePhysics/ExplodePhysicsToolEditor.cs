#if UNITY_EDITOR
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.MegaWorld.Editor.Common;
using VladislavTsurikov.MegaWorld.Editor.Common.PhysXPainter.Settings;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;
using VladislavTsurikov.PhysicsSimulator.Runtime.DisablePhysics;
using VladislavTsurikov.PhysicsSimulator.Runtime.Settings;

namespace VladislavTsurikov.MegaWorld.Editor.ExplodePhysics
{
    [ElementEditor(typeof(ExplodePhysicsTool))]
    public class ExplodePhysicsToolEditor : ToolWindowEditor
    {
	    public override void DrawButtons()
	    {
		    UndoEditor.DrawButtons(TargetType, WindowData.Instance.SelectedData);
	    }
	    
		public override void DrawFirstSettings()
		{
			PhysicsSimulatorSettingsEditor.OnGUI<ObjectTimeDisablePhysics>(PhysicsSimulatorSettings.Instance);
		}
    }
}
#endif