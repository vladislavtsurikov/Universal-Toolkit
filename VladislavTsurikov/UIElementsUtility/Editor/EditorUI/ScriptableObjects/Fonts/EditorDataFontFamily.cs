#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.UIElementsUtility.Editor.EditorUI.Enums;
using VladislavTsurikov.UIElementsUtility.Editor.EditorUI.Generators;
using VladislavTsurikov.UIElementsUtility.Editor.Utility;
using VladislavTsurikov.Utility.Runtime.Extensions;

namespace VladislavTsurikov.UIElementsUtility.Editor.EditorUI.ScriptableObjects.Fonts
{
    [
        CreateAssetMenu
        (
            fileName = DefaultAssetFilename,
            menuName = "Vladislav Tsurikov/EditorUI/Font Family"
        )
    ]
    public class EditorDataFontFamily : ScriptableObject
    {
        private const string DefaultAssetFilename = "_FontFamily";

        [SerializeField] private string _fontName;
        internal string FontName => _fontName;

        [SerializeField] private List<EditorFontInfo> _fonts = new List<EditorFontInfo>();
        internal List<EditorFontInfo> Fonts => _fonts;

        internal void SortFontsByWeight() =>
            _fonts = _fonts.OrderBy(fi => (int)fi.Weight).ToList();
        
        internal Font GetFont(int weightValue) =>
            GetFont((GenericFontWeight)weightValue);
        
        public static List<EditorDataFontFamily> GetGroups()
        {
            List<EditorDataFontFamily> groups = new List<EditorDataFontFamily>();
            
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(EditorDataFontFamily)}");
            
            foreach (string guid in guids)
            {
                EditorDataFontFamily group = AssetDatabase.LoadAssetAtPath<EditorDataFontFamily>(AssetDatabase.GUIDToAssetPath(guid));

                if (group != null)
                    groups.Add(group);
            }

            return groups;
        }
        
        public static EditorDataFontFamily GetGroup(string groupName)
        {
            foreach (var group in GetGroups())
            {
                if (group._fontName == groupName)
                    return group;
            }

            return null;
        }
        
        public void Setup(bool generateEditorLayout = false)
        {
            LoadFontsFromFolder(false);

            EditorUtility.SetDirty(this);
            
            if (generateEditorLayout)
                EditorFontsGenerator.Run();
        }

        private Font GetFont(GenericFontWeight weight)
        {
            _fonts = _fonts.Where(item => item != null && item.FontReference != null).ToList();

            foreach (EditorFontInfo fontInfo in _fonts.Where(fi => fi.Weight == weight))
                return fontInfo.FontReference;
            
            Debug.LogWarning($"Font '{FontName}-{weight}' not found! Returned default Unity font");
            
            return DesignUtils.unityDefaultFont;
        }

        public void LoadFontsFromFolder(bool saveAssets = true)
        {
            _fonts.Clear();
            string assetPath = AssetDatabase.GetAssetPath(this);
            string assetParentFolderPath = assetPath.Replace($"{name}.asset", "");
            string[] files = Directory.GetFiles(assetParentFolderPath, "*.ttf", SearchOption.TopDirectoryOnly);
            if (files.Length == 0)
            {
                // AssetDatabase.MoveAssetToTrash(assetPath);
                return;
            }

            foreach (string filePath in files)
            {
                Font reference = AssetDatabase.LoadAssetAtPath<Font>(filePath);
                if (reference == null) continue;
                _fonts.Add(new EditorFontInfo { FontReference = reference });
            }

            EditorUtility.SetDirty(this);
            Validate(saveAssets);

            string weights =
                Fonts
                    .Aggregate(string.Empty, (current, fontInfo) => current + $"{fontInfo.Weight}, ")
                    .TrimEnd(' ', ',');
        }

        internal void Validate(bool saveAssets = true)
        {
            string assetPath = AssetDatabase.GetAssetPath(this);
            string[] splitPath = assetPath.Split('/');
            string assetParentFolderName = splitPath[splitPath.Length - 2];

            _fontName = assetParentFolderName.RemoveAllSpecialCharacters().RemoveWhitespaces();
            AssetDatabase.RenameAsset(assetPath, $"{DefaultAssetFilename}_{_fontName}");

            if (_fonts == null) _fonts = new List<EditorFontInfo>();
            _fonts = _fonts.Where(fi => fi != null && fi.FontReference != null).ToList();
            foreach (EditorFontInfo fontInfo in Fonts)
            {
                GenericFontWeight weight = GenericFontWeight.Regular;
                foreach (GenericFontWeight style in Enum.GetValues(typeof(GenericFontWeight)))
                {
                    if (!fontInfo.FontReference.name.Contains($"-{style}")) continue;
                    weight = style;
                    break;
                }
                fontInfo.Weight = weight;
            }

            SortFontsByWeight();

            EditorUtility.SetDirty(this);
            if (saveAssets) AssetDatabase.SaveAssets();
        }
    }
}
#endif