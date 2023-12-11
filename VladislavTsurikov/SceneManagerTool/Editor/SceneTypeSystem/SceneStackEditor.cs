#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneTypeSystem;

namespace VladislavTsurikov.SceneManagerTool.Editor.SceneTypeSystem
{
    public class SceneStackEditor : ReorderableListStackEditor<SceneType, SceneTypeEditor>
    {
        public SceneStackEditor(GUIContent reorderableListName, ComponentStackSupportSameType<SceneType> list) : base(reorderableListName, list, true)
        {
            CopySettings = true;
            ShowActiveToggle = false;
        }
    }
}
#endif