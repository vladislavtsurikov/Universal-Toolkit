#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.UIElementsUtility.Editor.EditorUI;
using VladislavTsurikov.Utility.Runtime.Extensions;

namespace VladislavTsurikov.UIElementsUtility.Editor.Utility
{

    public static class DesignUtils
    {
        private static Font s_defaultFont;
        public static Font unityDefaultFont => s_defaultFont ? s_defaultFont : s_defaultFont = new VisualElement().GetStyleUnityFont();

        public const int k_FieldLabelFontSize = 9;
        public const int k_NormalLabelFontSize = 11;
        public const int k_Spacing = 4;
        public const int k_Spacing2X = k_Spacing * 2;
        public const int k_Spacing3X = k_Spacing * 3;
        public const int k_Spacing4X = k_Spacing * 4;
        public const int k_EndOfLineSpacing = k_Spacing * 6;
        public const int k_ToolbarHeight = 32;

#if !DISABLE_UIELEMENTS
        public static VisualElement GetSpaceBlock(int size, string name = "") =>
            GetSpaceBlock(size, size, name);

        public static VisualElement GetSpaceBlock(int width, int height, string name = "") =>
            new VisualElement()
                .SetName($"{name} Space Block ({width}x{height})")
                .SetStyleWidth(width)
                .SetStyleHeight(height)
                .SetStyleAlignSelf(Align.Center)
                .SetStyleFlexShrink(0);

        public static VisualElement endOfLineBlock => GetSpaceBlock(0, k_EndOfLineSpacing, "End of Line");
        public static VisualElement spaceBlock => GetSpaceBlock(k_Spacing, k_Spacing);
        public static VisualElement spaceBlock2X => GetSpaceBlock(k_Spacing2X, k_Spacing2X);
        public static VisualElement spaceBlock3X => GetSpaceBlock(k_Spacing3X, k_Spacing3X);
        public static VisualElement spaceBlock4X => GetSpaceBlock(k_Spacing4X, k_Spacing4X);

        public const int k_FieldBorderRadius = 6;

        public const int k_FieldNameTiny = 10;
        public const int k_FieldNameSmall = 10;
        public const int k_FieldNameNormal = 11;
        public const int k_FieldNameLarge = 11;
        public static Color fieldNameTextColor => EditorColors.Default.TextSubtitle;
        public static Font fieldNameTextFont => EditorFonts.Ubuntu.Light;
        

        /// <summary> Get a new VisualElement as a column </summary>
        public static VisualElement column =>
            new VisualElement().SetName("Column").SetStyleFlexDirection(FlexDirection.Column);

        /// <summary> Get a new VisualElement as a row </summary>
        public static VisualElement row =>
            new VisualElement().SetName("Row").SetStyleFlexDirection(FlexDirection.Row);

        /// <summary> Get a new VisualElement as empty flexible space </summary>
        public static VisualElement flexibleSpace =>
            new VisualElement()
                .SetName("Flexible Space")
                .SetStyleFlexGrow(1);

        public static Label fieldLabel =>
            new Label()
                .ResetLayout()
                .SetStyleUnityFont(fieldNameTextFont)
                .SetStyleFontSize(k_FieldLabelFontSize)
                .SetStyleColor(fieldNameTextColor);
#endif
    }
}
#endif