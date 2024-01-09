#if UNITY_EDITOR
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack.Attributes;
using VladislavTsurikov.MegaWorld.Editor.EditTool.ActionSystem.Elements;

namespace VladislavTsurikov.MegaWorld.Editor.EditTool.ActionSystem
{
    [CreateComponents(new []{typeof(MoveAlongDirection), typeof(Raycast), typeof(Rotate), typeof(Scale), typeof(Remove)})]
    public class ActionStack : ComponentStackOnlyDifferentTypes<Action>
    {
        public void CheckShortcutCombos()
        {
            foreach (Action settings in _elementList)
            {
                if(settings.CheckShortcutCombo())
                {
                    Select(settings);
                    return;
                }
            }
        }
    }
}
#endif