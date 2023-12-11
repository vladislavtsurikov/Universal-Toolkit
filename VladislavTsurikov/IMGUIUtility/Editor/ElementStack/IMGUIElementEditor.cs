#if UNITY_EDITOR
using VladislavTsurikov.ComponentStack.Editor;

namespace VladislavTsurikov.IMGUIUtility.Editor.ElementStack
{
    public class IMGUIElementEditor : ElementEditor
    {
        public virtual void OnGUI(){}
    }
}
#endif