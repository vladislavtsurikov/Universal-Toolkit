#if UNITY_EDITOR
using System;
using UnityEngine;
using VladislavTsurikov.ColorUtility.Runtime;
using VladislavTsurikov.Utility.Runtime;
using VladislavTsurikov.UIElementsUtility.Editor.Groups.EditorColors;

namespace VladislavTsurikov.UIElementsUtility.Editor.Groups.SelectableColors
{
    [Serializable]
    public class EditorSelectableColorInfo
    {
        public string ColorName;

        public EditorThemeColor Normal;
        public Color NormalColor => Normal.Color;

        public EditorThemeColor Highlighted;
        public Color HighlightedColor => Highlighted.Color;

        public EditorThemeColor Pressed;
        public Color PressedColor => Pressed.Color;

        public EditorThemeColor Selected;
        public Color SelectedColor => Selected.Color;

        public EditorThemeColor Disabled;
        public Color DisabledColor => Disabled.Color;

        public EditorSelectableColorInfo()
        {
            Normal = new EditorThemeColor();
            Highlighted = new EditorThemeColor();
            Pressed = new EditorThemeColor();
            Selected = new EditorThemeColor();
            Disabled = new EditorThemeColor();
        }

        public Color GetColor(SelectionState state)
        {
            switch (state)
            {
                case SelectionState.Normal: return NormalColor;
                case SelectionState.Highlighted: return HighlightedColor;
                case SelectionState.Pressed: return PressedColor;
                case SelectionState.Selected: return SelectedColor;
                case SelectionState.Disabled: return DisabledColor;
                default: throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        public EditorThemeColor GetThemeColor(SelectionState state)
        {
            switch (state)
            {
                case SelectionState.Normal: return Normal;
                case SelectionState.Highlighted: return Highlighted;
                case SelectionState.Pressed: return Pressed;
                case SelectionState.Selected: return Selected;
                case SelectionState.Disabled: return Disabled;
                default: throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        public EditorSelectableColorInfo GenerateAllColorVariantsFromNormalColor()
        {
            GenerateOnDarkColorVariantsFromNormalColor();
            GenerateOnLightColorVariantsFromNormalColor();
            return this;
        }

        public EditorSelectableColorInfo GenerateOnDarkColorVariantsFromNormalColor()
        {
            Highlighted.ColorOnDark = Normal.ColorOnDark.SetHSLHue(Normal.ColorOnDark.Hue() - 0.02f).WithRGBShade(0.2f);
            Pressed.ColorOnDark = Highlighted.ColorOnDark.WithRGBShade(0.2f);
            Selected.ColorOnDark = Highlighted.ColorOnDark.WithRGBShade(0.1f);
            Disabled.ColorOnDark = Normal.ColorOnDark.WithAlpha(0.6f);
            return this;
        }
        
        public EditorSelectableColorInfo GenerateOnLightColorVariantsFromNormalColor()
        {
            Highlighted.ColorOnLight = Normal.ColorOnLight.SetHSLHue(Normal.ColorOnLight.Hue() - 0.02f).WithRGBShade(0.2f);
            Pressed.ColorOnLight = Highlighted.ColorOnLight.WithRGBShade(0.1f);
            Selected.ColorOnLight = Highlighted.ColorOnLight.WithRGBTint(0.1f);
            Disabled.ColorOnLight = Normal.ColorOnLight.WithAlpha(0.6f);
            return this;
        }
        
        public void ValidateName() =>
            ColorName = ColorName.RemoveWhitespaces().RemoveAllSpecialCharacters();
    }
}
#endif