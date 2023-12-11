#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.UIElementsUtility.Editor.EditorUI.Generators;
using VladislavTsurikov.Utility.Runtime.Extensions;

namespace VladislavTsurikov.UIElementsUtility.Editor.EditorUI.ScriptableObjects.Layouts
{
    [
        CreateAssetMenu
        (
            fileName = DefaultAssetFilename,
            menuName = "Vladislav Tsurikov/EditorUI/Layout Group (UXML)"
        )
    ]
    public class EditorDataLayoutGroup : ScriptableObject
    {
        private const string DefaultAssetFilename = "_LayoutGroup";

        [SerializeField] private string _groupName;
        internal string GroupName => _groupName;

        [SerializeField] private List<EditorLayoutInfo> _layouts = new List<EditorLayoutInfo>();
        internal List<EditorLayoutInfo> Layouts => _layouts;
        
        public static List<EditorDataLayoutGroup> GetGroups()
        {
            List<EditorDataLayoutGroup> editorDataLayoutGroups = new List<EditorDataLayoutGroup>();
            
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(EditorDataLayoutGroup)}");
            
            foreach (string guid in guids)
            {
                EditorDataLayoutGroup editorDataLayoutGroup = AssetDatabase.LoadAssetAtPath<EditorDataLayoutGroup>(AssetDatabase.GUIDToAssetPath(guid));

                if (editorDataLayoutGroup != null)
                    editorDataLayoutGroups.Add(editorDataLayoutGroup);
            }

            return editorDataLayoutGroups;
        }

        public static EditorDataLayoutGroup GetGroup(string groupName)
        {
            foreach (var group in GetGroups())
            {
                if (group.GroupName == groupName)
                    return group;
            }

            return null;
        }
        
        public void Setup(bool generateEditorLayout = false)
        {
            LoadUxmlReferencesFromFolder(false);

            EditorUtility.SetDirty(this);
            
            if (generateEditorLayout)
                EditorLayoutsGenerator.Run();
        }

        internal void AddNewItem() =>
            _layouts.Insert(0, new EditorLayoutInfo());

        internal void SortByFileName()
        {
            _layouts = _layouts.Where(item => item != null && item.UxmlReference != null).ToList();
            _layouts = _layouts.OrderBy(item => item.UxmlReference.name).ToList();
        }

        internal VisualTreeAsset GetVisualTreeAsset(string layoutName)
        {
            string cleanName = layoutName.RemoveWhitespaces().RemoveAllSpecialCharacters();

            _layouts = _layouts.Where(item => item != null && item.UxmlReference != null).ToList();

            foreach (EditorLayoutInfo layoutInfo in _layouts.Where(item => item.UxmlReference.name.RemoveWhitespaces().RemoveAllSpecialCharacters().Equals(cleanName)))
                return layoutInfo.UxmlReference;

            Debug.LogWarning($"UXML Layout '{layoutName}' not found! Returned null");
            return null;
        }

        public void LoadUxmlReferencesFromFolder(bool saveAssets = true)
        {
            _layouts.Clear();
            string assetPath = AssetDatabase.GetAssetPath(this);
            string assetParentFolderPath = assetPath.Replace($"{name}.asset", "");
            string[] files = Directory.GetFiles(assetParentFolderPath, "*.uxml", SearchOption.TopDirectoryOnly);
            if (files.Length == 0)
            {
                // AssetDatabase.MoveAssetToTrash(assetPath);
                return;
            }

            foreach (string filePath in files)
            {
                VisualTreeAsset reference = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(filePath);
                if (reference == null) continue;
                _layouts.Add(new EditorLayoutInfo { UxmlReference = reference });
            }
            
            Debug.Log($"Found the '{GroupName}' UXML Layout Group ({_layouts.Count} layouts)");
        }
    }
}
#endif