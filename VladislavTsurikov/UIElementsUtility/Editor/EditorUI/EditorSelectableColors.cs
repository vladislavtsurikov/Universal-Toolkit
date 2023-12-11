//.........................
//.....Generated File......
//.........................
//.......Do not edit.......
//.........................

#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.UIElementsUtility.Editor.EditorUI.ScriptableObjects.Colors;
using VladislavTsurikov.UIElementsUtility.Runtime.Color;

namespace VladislavTsurikov.UIElementsUtility.Editor.EditorUI
{
    public static class EditorSelectableColors
    {
        public static class Default
        {
            public enum ColorName
            {
                ButtonContainer,
                ButtonIcon,
                ButtonText,
                Action,
                Add,
                Remove
            }

            private static EditorDataSelectableColorPalette s_selectableColorPalette;
            private static EditorSelectableColorInfo s_ButtonContainer;
            private static EditorSelectableColorInfo s_ButtonIcon;
            private static EditorSelectableColorInfo s_ButtonText;
            private static EditorSelectableColorInfo s_Action;
            private static EditorSelectableColorInfo s_Add;
            private static EditorSelectableColorInfo s_Remove;

            private static EditorDataSelectableColorPalette selectableColorPalette { get; } = s_selectableColorPalette != null? s_selectableColorPalette: s_selectableColorPalette = EditorDataSelectableColorPalette.GetGroup("Default");

            public static EditorSelectableColorInfo ButtonContainer { get; } = s_ButtonContainer ?? (s_ButtonContainer = GetSelectableColorInfo(ColorName.ButtonContainer));

            public static EditorSelectableColorInfo ButtonIcon { get; } = s_ButtonIcon ?? (s_ButtonIcon = GetSelectableColorInfo(ColorName.ButtonIcon));

            public static EditorSelectableColorInfo ButtonText { get; } = s_ButtonText ?? (s_ButtonText = GetSelectableColorInfo(ColorName.ButtonText));

            public static EditorSelectableColorInfo Action { get; } = s_Action ?? (s_Action = GetSelectableColorInfo(ColorName.Action));

            public static EditorSelectableColorInfo Add { get; } = s_Add ?? (s_Add = GetSelectableColorInfo(ColorName.Add));

            public static EditorSelectableColorInfo Remove { get; } = s_Remove ?? (s_Remove = GetSelectableColorInfo(ColorName.Remove));

            public static Color GetColor(ColorName colorName, SelectionState state)
            {
                return selectableColorPalette.GetColor(colorName.ToString(), state);
            }

            public static EditorThemeColor GetThemeColor(ColorName colorName, SelectionState state)
            {
                return selectableColorPalette.GetThemeColor(colorName.ToString(), state);
            }

            public static EditorSelectableColorInfo GetSelectableColorInfo(ColorName colorName)
            {
                return selectableColorPalette.GetSelectableColorInfo(colorName.ToString());
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
                DeepOrange
            }

            private static EditorDataSelectableColorPalette s_selectableColorPalette;
            private static EditorSelectableColorInfo s_Red;
            private static EditorSelectableColorInfo s_Pink;
            private static EditorSelectableColorInfo s_Purple;
            private static EditorSelectableColorInfo s_DeepPurple;
            private static EditorSelectableColorInfo s_Indigo;
            private static EditorSelectableColorInfo s_Blue;
            private static EditorSelectableColorInfo s_LightBlue;
            private static EditorSelectableColorInfo s_Cyan;
            private static EditorSelectableColorInfo s_Teal;
            private static EditorSelectableColorInfo s_Green;
            private static EditorSelectableColorInfo s_LightGreen;
            private static EditorSelectableColorInfo s_Lime;
            private static EditorSelectableColorInfo s_Yellow;
            private static EditorSelectableColorInfo s_Amber;
            private static EditorSelectableColorInfo s_Orange;
            private static EditorSelectableColorInfo s_DeepOrange;

            private static EditorDataSelectableColorPalette selectableColorPalette { get; } = s_selectableColorPalette != null? s_selectableColorPalette: s_selectableColorPalette = EditorDataSelectableColorPalette.GetGroup("EditorUI");

            public static EditorSelectableColorInfo Red { get; } = s_Red ?? (s_Red = GetSelectableColorInfo(ColorName.Red));

            public static EditorSelectableColorInfo Pink { get; } = s_Pink ?? (s_Pink = GetSelectableColorInfo(ColorName.Pink));

            public static EditorSelectableColorInfo Purple { get; } = s_Purple ?? (s_Purple = GetSelectableColorInfo(ColorName.Purple));

            public static EditorSelectableColorInfo DeepPurple { get; } = s_DeepPurple ?? (s_DeepPurple = GetSelectableColorInfo(ColorName.DeepPurple));

            public static EditorSelectableColorInfo Indigo { get; } = s_Indigo ?? (s_Indigo = GetSelectableColorInfo(ColorName.Indigo));

            public static EditorSelectableColorInfo Blue { get; } = s_Blue ?? (s_Blue = GetSelectableColorInfo(ColorName.Blue));

            public static EditorSelectableColorInfo LightBlue { get; } = s_LightBlue ?? (s_LightBlue = GetSelectableColorInfo(ColorName.LightBlue));

            public static EditorSelectableColorInfo Cyan { get; } = s_Cyan ?? (s_Cyan = GetSelectableColorInfo(ColorName.Cyan));

            public static EditorSelectableColorInfo Teal { get; } = s_Teal ?? (s_Teal = GetSelectableColorInfo(ColorName.Teal));

            public static EditorSelectableColorInfo Green { get; } = s_Green ?? (s_Green = GetSelectableColorInfo(ColorName.Green));

            public static EditorSelectableColorInfo LightGreen { get; } = s_LightGreen ?? (s_LightGreen = GetSelectableColorInfo(ColorName.LightGreen));

            public static EditorSelectableColorInfo Lime { get; } = s_Lime ?? (s_Lime = GetSelectableColorInfo(ColorName.Lime));

            public static EditorSelectableColorInfo Yellow { get; } = s_Yellow ?? (s_Yellow = GetSelectableColorInfo(ColorName.Yellow));

            public static EditorSelectableColorInfo Amber { get; } = s_Amber ?? (s_Amber = GetSelectableColorInfo(ColorName.Amber));

            public static EditorSelectableColorInfo Orange { get; } = s_Orange ?? (s_Orange = GetSelectableColorInfo(ColorName.Orange));

            public static EditorSelectableColorInfo DeepOrange { get; } = s_DeepOrange ?? (s_DeepOrange = GetSelectableColorInfo(ColorName.DeepOrange));

            public static Color GetColor(ColorName colorName, SelectionState state)
            {
                return selectableColorPalette.GetColor(colorName.ToString(), state);
            }

            public static EditorThemeColor GetThemeColor(ColorName colorName, SelectionState state)
            {
                return selectableColorPalette.GetThemeColor(colorName.ToString(), state);
            }

            public static EditorSelectableColorInfo GetSelectableColorInfo(ColorName colorName)
            {
                return selectableColorPalette.GetSelectableColorInfo(colorName.ToString());
            }
        }
    }
}
#endif
