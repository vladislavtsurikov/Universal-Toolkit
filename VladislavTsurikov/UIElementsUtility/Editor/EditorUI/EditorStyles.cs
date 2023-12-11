//.........................
//.....Generated File......
//.........................
//.......Do not edit.......
//.........................

#if UNITY_EDITOR
using UnityEngine.UIElements;
using VladislavTsurikov.UIElementsUtility.Editor.EditorUI.ScriptableObjects.Styles;

namespace VladislavTsurikov.UIElementsUtility.Editor.EditorUI
{
    public static class EditorStyles
    {
        public static class EditorUI
        {
            public enum StyleName
            {
                Button,
                ListView,
                ListViewItem
            }

            private static EditorDataStyleGroup s_styleGroup;
            private static StyleSheet s_Button;
            private static StyleSheet s_ListView;
            private static StyleSheet s_ListViewItem;

            private static EditorDataStyleGroup styleGroup { get; } = s_styleGroup != null? s_styleGroup: s_styleGroup = EditorDataStyleGroup.GetGroup("EditorUI");

            public static StyleSheet Button { get; } = s_Button ? s_Button : s_Button = GetStyleSheet(StyleName.Button);

            public static StyleSheet ListView { get; } = s_ListView ? s_ListView : s_ListView = GetStyleSheet(StyleName.ListView);

            public static StyleSheet ListViewItem { get; } = s_ListViewItem ? s_ListViewItem : s_ListViewItem = GetStyleSheet(StyleName.ListViewItem);

            public static StyleSheet GetStyleSheet(StyleName styleName)
            {
                return styleGroup.GetStyleSheet(styleName.ToString());
            }
        }
    }
}
#endif
