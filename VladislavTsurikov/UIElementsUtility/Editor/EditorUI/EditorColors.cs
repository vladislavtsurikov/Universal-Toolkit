//.........................
//.....Generated File......
//.........................
//.......Do not edit.......
//.........................

#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.UIElementsUtility.Editor.EditorUI.ScriptableObjects.Colors;

namespace VladislavTsurikov.UIElementsUtility.Editor.EditorUI
{
    public static class EditorColors
    {
        public static class Default
        {
            public enum ColorName
            {
                Add,
                Remove,
                Selection,
                TextTitle,
                FieldIcon,
                TextSubtitle,
                TextDescription,
                Background
            }

            private static EditorDataColorPalette s_colorPalette;
            private static Color? s_Add;
            private static Color? s_Remove;
            private static Color? s_Selection;
            private static Color? s_TextTitle;
            private static Color? s_FieldIcon;
            private static Color? s_TextSubtitle;
            private static Color? s_TextDescription;
            private static Color? s_Background;

            private static EditorDataColorPalette colorPalette { get; } = s_colorPalette != null? s_colorPalette: s_colorPalette = EditorDataColorPalette.GetGroup("Default");

            public static Color Add { get; } = (Color) (s_Add ?? (s_Add = GetColor(ColorName.Add)));

            public static Color Remove { get; } = (Color) (s_Remove ?? (s_Remove = GetColor(ColorName.Remove)));

            public static Color Selection { get; } = (Color) (s_Selection ?? (s_Selection = GetColor(ColorName.Selection)));

            public static Color TextTitle { get; } = (Color) (s_TextTitle ?? (s_TextTitle = GetColor(ColorName.TextTitle)));

            public static Color FieldIcon { get; } = (Color) (s_FieldIcon ?? (s_FieldIcon = GetColor(ColorName.FieldIcon)));

            public static Color TextSubtitle { get; } = (Color) (s_TextSubtitle ?? (s_TextSubtitle = GetColor(ColorName.TextSubtitle)));

            public static Color TextDescription { get; } = (Color) (s_TextDescription ?? (s_TextDescription = GetColor(ColorName.TextDescription)));

            public static Color Background { get; } = (Color) (s_Background ?? (s_Background = GetColor(ColorName.Background)));

            public static Color GetColor(ColorName colorName)
            {
                return colorPalette.GetColor(colorName.ToString());
            }
        }

        public static class EditorUI
        {
            public enum ColorName
            {
                Red,
                Pink,
                Purple,
                DeepPurple,
                Indigo,
                Blue,
                LightBlue,
                Cyan,
                Teal,
                Green,
                LightGreen,
                Lime,
                Yellow,
                Amber,
                Orange,
                DeepOrange,
                Black,
                White,
                Gray
            }

            private static EditorDataColorPalette s_colorPalette;
            private static Color? s_Red;
            private static Color? s_Pink;
            private static Color? s_Purple;
            private static Color? s_DeepPurple;
            private static Color? s_Indigo;
            private static Color? s_Blue;
            private static Color? s_LightBlue;
            private static Color? s_Cyan;
            private static Color? s_Teal;
            private static Color? s_Green;
            private static Color? s_LightGreen;
            private static Color? s_Lime;
            private static Color? s_Yellow;
            private static Color? s_Amber;
            private static Color? s_Orange;
            private static Color? s_DeepOrange;
            private static Color? s_Black;
            private static Color? s_White;
            private static Color? s_Gray;

            private static EditorDataColorPalette colorPalette { get; } = s_colorPalette != null? s_colorPalette: s_colorPalette = EditorDataColorPalette.GetGroup("EditorUI");

            public static Color Red { get; } = (Color) (s_Red ?? (s_Red = GetColor(ColorName.Red)));

            public static Color Pink { get; } = (Color) (s_Pink ?? (s_Pink = GetColor(ColorName.Pink)));

            public static Color Purple { get; } = (Color) (s_Purple ?? (s_Purple = GetColor(ColorName.Purple)));

            public static Color DeepPurple { get; } = (Color) (s_DeepPurple ?? (s_DeepPurple = GetColor(ColorName.DeepPurple)));

            public static Color Indigo { get; } = (Color) (s_Indigo ?? (s_Indigo = GetColor(ColorName.Indigo)));

            public static Color Blue { get; } = (Color) (s_Blue ?? (s_Blue = GetColor(ColorName.Blue)));

            public static Color LightBlue { get; } = (Color) (s_LightBlue ?? (s_LightBlue = GetColor(ColorName.LightBlue)));

            public static Color Cyan { get; } = (Color) (s_Cyan ?? (s_Cyan = GetColor(ColorName.Cyan)));

            public static Color Teal { get; } = (Color) (s_Teal ?? (s_Teal = GetColor(ColorName.Teal)));

            public static Color Green { get; } = (Color) (s_Green ?? (s_Green = GetColor(ColorName.Green)));

            public static Color LightGreen { get; } = (Color) (s_LightGreen ?? (s_LightGreen = GetColor(ColorName.LightGreen)));

            public static Color Lime { get; } = (Color) (s_Lime ?? (s_Lime = GetColor(ColorName.Lime)));

            public static Color Yellow { get; } = (Color) (s_Yellow ?? (s_Yellow = GetColor(ColorName.Yellow)));

            public static Color Amber { get; } = (Color) (s_Amber ?? (s_Amber = GetColor(ColorName.Amber)));

            public static Color Orange { get; } = (Color) (s_Orange ?? (s_Orange = GetColor(ColorName.Orange)));

            public static Color DeepOrange { get; } = (Color) (s_DeepOrange ?? (s_DeepOrange = GetColor(ColorName.DeepOrange)));

            public static Color Black { get; } = (Color) (s_Black ?? (s_Black = GetColor(ColorName.Black)));

            public static Color White { get; } = (Color) (s_White ?? (s_White = GetColor(ColorName.White)));

            public static Color Gray { get; } = (Color) (s_Gray ?? (s_Gray = GetColor(ColorName.Gray)));

            public static Color GetColor(ColorName colorName)
            {
                return colorPalette.GetColor(colorName.ToString());
            }
        }
    }
}
#endif
