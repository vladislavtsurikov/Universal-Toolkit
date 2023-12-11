#if UNITY_EDITOR
using VladislavTsurikov.ComponentStack.Editor.Attributes;
using VladislavTsurikov.MegaWorld.Editor.Common;
using VladislavTsurikov.MegaWorld.Editor.Common.Settings.PhysicsToolsSettings;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;
using VladislavTsurikov.MegaWorld.Editor.Core.Window.ElementSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.PhysicsSimulatorEditor.Editor;

namespace VladislavTsurikov.MegaWorld.Editor.PhysicsEffectsTool.GUI
{
    [ElementEditor(typeof(PhysicsEffectsTool))]
    public class PhysicsEffectsToolEditor : ToolWindowEditor
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
		    
		    PhysicsEffectsToolSettingsEditor editor = (PhysicsEffectsToolSettingsEditor)ToolsComponentStackEditor.GetEditor(typeof(PhysicsEffectsTool), typeof(PhysicsEffectsToolSettings));
		    editor.PhysicsEffectStackEditor.DrawButtons();
	    }
	    
	    public override void DrawFirstSettings()
	    {
		    PhysicsSimulatorSettingsEditor.OnGUI(PhysicsSimulatorSettings.Instance, DisablePhysicsMode.GlobalTime, false);
	    }
    }
}
#endif