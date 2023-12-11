//.........................
//.....Generated File......
//.........................
//.......Do not edit.......
//.........................

#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.UIElementsUtility.Editor.EditorUI.ScriptableObjects.Fonts;

namespace VladislavTsurikov.UIElementsUtility.Editor.EditorUI
{
    public static class EditorFonts
    {
        public static class Inter
        {
            public enum FontWeight
            {
                Thin = 0,
                ExtraLight = 1,
                Light = 2,
                Regular = 3,
                Medium = 4,
                SemiBold = 5,
                Bold = 6,
                ExtraBold = 7,
                Black = 8
            }

            private static EditorDataFontFamily s_fontFamily;
            private static Font s_Thin;
            private static Font s_ExtraLight;
            private static Font s_Light;
            private static Font s_Regular;
            private static Font s_Medium;
            private static Font s_SemiBold;
            private static Font s_Bold;
            private static Font s_ExtraBold;
            private static Font s_Black;

            private static EditorDataFontFamily fontFamily { get; } = s_fontFamily != null? s_fontFamily: s_fontFamily = EditorDataFontFamily.GetGroup("Inter");

            public static Font Thin { get; } = s_Thin ? s_Thin : s_Thin = GetFont(FontWeight.Thin);

            public static Font ExtraLight { get; } = s_ExtraLight ? s_ExtraLight : s_ExtraLight = GetFont(FontWeight.ExtraLight);

            public static Font Light { get; } = s_Light ? s_Light : s_Light = GetFont(FontWeight.Light);

            public static Font Regular { get; } = s_Regular ? s_Regular : s_Regular = GetFont(FontWeight.Regular);

            public static Font Medium { get; } = s_Medium ? s_Medium : s_Medium = GetFont(FontWeight.Medium);

            public static Font SemiBold { get; } = s_SemiBold ? s_SemiBold : s_SemiBold = GetFont(FontWeight.SemiBold);

            public static Font Bold { get; } = s_Bold ? s_Bold : s_Bold = GetFont(FontWeight.Bold);

            public static Font ExtraBold { get; } = s_ExtraBold ? s_ExtraBold : s_ExtraBold = GetFont(FontWeight.ExtraBold);

            public static Font Black { get; } = s_Black ? s_Black : s_Black = GetFont(FontWeight.Black);

            public static Font GetFont(FontWeight weight)
            {
                return fontFamily.GetFont((int)weight);
            }
        }

        public static class Ubuntu
        {
            public enum FontWeight
            {
                Light = 2,
                Regular = 3,
                Medium = 4,
                Bold = 6
            }

            private static EditorDataFontFamily s_fontFamily;
            private static Font s_Light;
            private static Font s_Regular;
            private static Font s_Medium;
            private static Font s_Bold;

            private static EditorDataFontFamily fontFamily { get; } = s_fontFamily != null? s_fontFamily: s_fontFamily = EditorDataFontFamily.GetGroup("Ubuntu");

            public static Font Light { get; } = s_Light ? s_Light : s_Light = GetFont(FontWeight.Light);

            public static Font Regular { get; } = s_Regular ? s_Regular : s_Regular = GetFont(FontWeight.Regular);

            public static Font Medium { get; } = s_Medium ? s_Medium : s_Medium = GetFont(FontWeight.Medium);

            public static Font Bold { get; } = s_Bold ? s_Bold : s_Bold = GetFont(FontWeight.Bold);

            public static Font GetFont(FontWeight weight)
            {
                return fontFamily.GetFont((int)weight);
            }
        }
    }
}
#endif
