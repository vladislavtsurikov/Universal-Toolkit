//.........................
//.....Generated File......
//.........................
//.......Do not edit.......
//.........................

#if UNITY_EDITOR
using UnityEngine.UIElements;
using VladislavTsurikov.UIElementsUtility.Editor.EditorUI.ScriptableObjects.Layouts;

namespace VladislavTsurikov.UIElementsUtility.Editor.EditorUI
{
    public static class EditorLayouts
    {
        public static class EditorUI
        {
            public enum LayoutName
            {
                Button,
                ListView,
                ListViewItem
            }

            private static EditorDataLayoutGroup s_layoutGroup;
            private static VisualTreeAsset s_Button;
            private static VisualTreeAsset s_ListView;
            private static VisualTreeAsset s_ListViewItem;

            private static EditorDataLayoutGroup layoutGroup { get; } = s_layoutGroup != null? s_layoutGroup: s_layoutGroup = EditorDataLayoutGroup.GetGroup("EditorUI");

            public static VisualTreeAsset Button { get; } = s_Button ? s_Button : s_Button = GetVisualTreeAsset(LayoutName.Button);

            public static VisualTreeAsset ListView { get; } = s_ListView ? s_ListView : s_ListView = GetVisualTreeAsset(LayoutName.ListView);

            public static VisualTreeAsset ListViewItem { get; } = s_ListViewItem ? s_ListViewItem : s_ListViewItem = GetVisualTreeAsset(LayoutName.ListViewItem);

            public static VisualTreeAsset GetVisualTreeAsset(LayoutName layoutName)
            {
                return layoutGroup.GetVisualTreeAsset(layoutName.ToString());
            }
        }
    }
}
#endif
