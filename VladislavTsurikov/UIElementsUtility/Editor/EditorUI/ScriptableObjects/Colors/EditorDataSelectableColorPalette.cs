#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ColorUtility.Runtime;
using VladislavTsurikov.UIElementsUtility.Editor.EditorUI.Generators;
using VladislavTsurikov.UIElementsUtility.Runtime.Color;
using VladislavTsurikov.Utility.Runtime.Extensions;

namespace VladislavTsurikov.UIElementsUtility.Editor.EditorUI.ScriptableObjects.Colors
{
    [
        CreateAssetMenu
        (
            fileName = DEFAULT_ASSET_FILENAME,
            menuName = "Vladislav Tsurikov/EditorUI/Selectable Color Palette"
        )
    ]
    public class EditorDataSelectableColorPalette : ScriptableObject
    {
        private const string DEFAULT_ASSET_FILENAME = "_SelectableColorPalette";
        private static string AssetFileName(string paletteName) => $"{DEFAULT_ASSET_FILENAME}_{paletteName.RemoveWhitespaces()}";

        [SerializeField] private string PaletteName;
        internal string paletteName => PaletteName;

        [SerializeField] private List<EditorSelectableColorInfo> SelectableColors = new List<EditorSelectableColorInfo>();
        internal List<EditorSelectableColorInfo> selectableColors => SelectableColors;

        internal void AddNewItem() =>
            SelectableColors.Insert(0, new EditorSelectableColorInfo());

        internal void SortByColorName() =>
            SelectableColors = SelectableColors.OrderBy(item => item.ColorName).ToList();

        internal void SortByHue() =>
            SelectableColors = SelectableColors.OrderByDescending(item => item.Normal.ColorOnDark.Hue()).ToList();
        
        public static List<EditorDataSelectableColorPalette> GetGroups()
        {
            List<EditorDataSelectableColorPalette> groups = new List<EditorDataSelectableColorPalette>();
            
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(EditorDataSelectableColorPalette)}");
            
            foreach (string guid in guids)
            {
                EditorDataSelectableColorPalette group = AssetDatabase.LoadAssetAtPath<EditorDataSelectableColorPalette>(AssetDatabase.GUIDToAssetPath(guid));

                if (group != null)
                    groups.Add(group);
            }

            return groups;
        }
        
        public static EditorDataSelectableColorPalette GetGroup(string groupName)
        {
            foreach (var group in GetGroups())
            {
                if (group.PaletteName == groupName)
                    return group;
            }

            return null;
        }

        // [MenuItem("Window/Vladislav Tsurikov/UIElements/Generate All Color Variants From Normal Color", false, 0)]
        // public static void GenerateAllColorVariantsFromNormalColor()
        // {
        //     foreach (var group in GetGroups())
        //     {
        //         foreach (var selectableColor in group.SelectableColors)
        //         {
        //             selectableColor.GenerateAllColorVariantsFromNormalColor();
        //         }
        //     }
        // }

        public static void RegenerateDefaultSelectableColorPalette()
        {
            const string defaultColorPaletteName = "EditorUI";
            string defaultColorPalettePath = $"{EditorPath.Path}/EditorUI/Colors/{AssetFileName(defaultColorPaletteName)}.asset";

            EditorDataSelectableColorPalette data = AssetDatabase.LoadAssetAtPath<EditorDataSelectableColorPalette>(defaultColorPalettePath);
            bool defaultPaletteNotFound = data == null;
            if (!defaultPaletteNotFound) return;
            data = CreateInstance<EditorDataSelectableColorPalette>();
            data.PaletteName = defaultColorPaletteName;
            data.name = AssetFileName(defaultColorPaletteName);
            data.SelectableColors = new List<EditorSelectableColorInfo>();
            EditorDataColorPalette defaultColorPalette = EditorDataColorPalette.GetGroup("EditorUI");
            foreach (EditorColorInfo colorInfo in defaultColorPalette.colors)
            {
                if (colorInfo.ColorName.Equals("Black")) continue;
                if (colorInfo.ColorName.Equals("White")) continue;
                if (colorInfo.ColorName.Equals("Gray")) continue;
                data.SelectableColors.Add
                (
                    new EditorSelectableColorInfo
                        {
                            ColorName = colorInfo.ColorName.RemoveWhitespaces().RemoveAllSpecialCharacters(),
                            Normal = new EditorThemeColor
                            {
                                ColorOnDark = colorInfo.ThemeColor.ColorOnDark,
                                ColorOnLight = colorInfo.ThemeColor.ColorOnLight
                            }
                        }
                        .GenerateAllColorVariantsFromNormalColor()
                );
            }
            data.SortByHue();

            AssetDatabase.CreateAsset(data, defaultColorPalettePath);
            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public void Setup(bool generateEditorLayout = false)
        {
            EditorUtility.SetDirty(this);

            if (generateEditorLayout)
                EditorSelectableColorsGenerator.Run();
        }

        internal EditorSelectableColorInfo GetSelectableColorInfo(string colorName, bool silent = false)
        {
            string cleanName = colorName.RemoveWhitespaces().RemoveAllSpecialCharacters();

            SelectableColors = SelectableColors.Where(item => item != null).ToList();

            foreach (EditorSelectableColorInfo selectableColorInfo in SelectableColors.Where(item => item.ColorName.Equals(cleanName)))
                return selectableColorInfo;

            if (!silent)
            {
                Debug.LogWarning($"SelectableColor '{colorName}' not found! Returned null");
            }

            return null;
        }

        internal Color GetColor(string colorName, SelectionState state)
        {
            EditorSelectableColorInfo selectableColorInfo = GetSelectableColorInfo(colorName);
            if (selectableColorInfo != null)
                return selectableColorInfo.GetColor(state);
            Debug.LogWarning($"SelectableColor '{colorName}' not found! Returned Color.magenta");
            return Color.magenta;
        }

        internal EditorThemeColor GetThemeColor(string colorName, SelectionState state)
        {
            EditorSelectableColorInfo selectableColorInfo = GetSelectableColorInfo(colorName);
            if (selectableColorInfo != null)
                return selectableColorInfo.GetThemeColor(state);
            Debug.LogWarning($"SelectableColor '{colorName}' not found! Returned null");
            return null;
        }
    }
}
#endif