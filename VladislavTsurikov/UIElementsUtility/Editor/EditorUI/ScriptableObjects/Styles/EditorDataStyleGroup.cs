#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.UIElementsUtility.Editor.EditorUI.Generators;
using VladislavTsurikov.Utility.Runtime.Extensions;

namespace VladislavTsurikov.UIElementsUtility.Editor.EditorUI.ScriptableObjects.Styles
{
    [
        CreateAssetMenu
        (
            fileName = DefaultAssetFilename,
            menuName = "Vladislav Tsurikov/EditorUI/Style Group (USS)"
        )
    ]
    public class EditorDataStyleGroup : ScriptableObject
    {
        private const string DefaultAssetFilename = "_StyleGroup";

        [SerializeField] private string _groupName;
        internal string GroupName => _groupName;

        [SerializeField] private List<EditorStyleInfo> _styles = new List<EditorStyleInfo>();
        internal List<EditorStyleInfo> Styles => _styles;
        
        public static List<EditorDataStyleGroup> GetGroups()
        {
            List<EditorDataStyleGroup> groups = new List<EditorDataStyleGroup>();
            
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(EditorDataStyleGroup)}");

            foreach (string guid in guids)
            {
                EditorDataStyleGroup group = AssetDatabase.LoadAssetAtPath<EditorDataStyleGroup>(AssetDatabase.GUIDToAssetPath(guid));

                if (group != null)
                    groups.Add(group);
            }
            
            return groups;
        }
        
        public static EditorDataStyleGroup GetGroup(string groupName)
        {
            foreach (var group in GetGroups())
            {
                if (group.GroupName == groupName)
                    return group;
            }

            return null;
        }
        
        public void Setup(bool generateEditorStyles = false)
        {
            LoadUssReferencesFromFolder(false);

            EditorUtility.SetDirty(this);

            if (generateEditorStyles)
                EditorStylesGenerator.Run();
        }

        internal void AddNewItem() =>
            _styles.Insert(0, new EditorStyleInfo());
        
        internal void SortByFileName() =>
            _styles = _styles.OrderBy(item => item.UssReference.name).ToList();
        
        internal StyleSheet GetStyleSheet(string styleName)
        {
            string cleanName = styleName.RemoveWhitespaces().RemoveAllSpecialCharacters();

            _styles = _styles.Where(item => item != null && item.UssReference != null).ToList();
            
            foreach (EditorStyleInfo styleInfo in _styles.Where(item => item.UssReference.name.RemoveWhitespaces().RemoveAllSpecialCharacters().Equals(cleanName)))
                return styleInfo.UssReference;

            Debug.LogWarning($"USS Style '{styleName}' not found! Returned null");
            return null;
        }

        public void LoadUssReferencesFromFolder(bool saveAssets = true)
        {
            _styles.Clear();
            string assetPath = AssetDatabase.GetAssetPath(this);
            string assetParentFolderPath = assetPath.Replace($"{name}.asset", "");
            string[] files = Directory.GetFiles(assetParentFolderPath, "*.uss", SearchOption.TopDirectoryOnly);
            if (files.Length == 0)
            {
                return;
            }

            foreach (string filePath in files)
            {
                StyleSheet reference = AssetDatabase.LoadAssetAtPath<StyleSheet>(filePath);
                if (reference == null) continue;
                _styles.Add(new EditorStyleInfo { UssReference = reference });
            }
        }
    }
}
#endif