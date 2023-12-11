#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.UIElementsUtility.Editor.EditorUI.Generators;
using VladislavTsurikov.UIElementsUtility.Editor.Utility;
using VladislavTsurikov.Utility.Runtime.Extensions;

namespace VladislavTsurikov.UIElementsUtility.Editor.EditorUI.ScriptableObjects.Textures
{
    [
        CreateAssetMenu
        (
            fileName = DefaultAssetFilename,
            menuName = "Vladislav Tsurikov/EditorUI/Texture Group"
        )
    ]
    public class EditorDataTextureGroup : ScriptableObject
    {
        private const string DefaultAssetFilename = "_TextureGroup";

        [SerializeField] private string _groupCategory;
        internal string GroupCategory => _groupCategory;
        
        [SerializeField] private string _groupName;
        internal string GroupName => _groupName;

        [SerializeField] private string _removePrefixFromTextureName;
        internal string RemovePrefixFromTextureName => _removePrefixFromTextureName;
        
        [SerializeField]
        private List<EditorTextureInfo> _textures = new List<EditorTextureInfo>();
        internal List<EditorTextureInfo> Textures => _textures;

        internal void AddNewItem() =>
            _textures.Insert(0, new EditorTextureInfo());

        internal void RemoveNullEntries() =>
            _textures = _textures.Where(item => item != null && item.TextureReference != null).ToList();

        internal void SortByFileName() =>
            _textures = _textures.OrderBy(item => item.TextureReference.name).ToList();

        internal void RemoveDuplicates() =>
            _textures = _textures.GroupBy(item => item.TextureReference).Select(item => item.First()).ToList();

        internal Texture2D GetTexture(string textureName)
        {
            string cleanName = textureName.RemoveWhitespaces().RemoveAllSpecialCharacters();
            
            // foreach (EditorTextureInfo textureInfo in textures.Where(ti => ti.TextureName.RemoveWhitespaces().RemoveAllSpecialCharacters().Equals(cleanName)))
            foreach (EditorTextureInfo textureInfo in Textures.Where(ti => ti.TextureName.Equals(cleanName)))
                return textureInfo.TextureReference;
            
            Debug.LogWarning($"Texture '{textureName}' not found! Returned null.");
            
            return null;
        }
        
        public static List<EditorDataTextureGroup> GetGroups()
        {
            List<EditorDataTextureGroup> editorDataLayoutGroups = new List<EditorDataTextureGroup>();
            
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(EditorDataTextureGroup)}");
            
            foreach (string guid in guids)
            {
                EditorDataTextureGroup editorDataLayoutGroup = AssetDatabase.LoadAssetAtPath<EditorDataTextureGroup>(AssetDatabase.GUIDToAssetPath(guid));

                if (editorDataLayoutGroup != null)
                    editorDataLayoutGroups.Add(editorDataLayoutGroup);
            }

            return editorDataLayoutGroups;
        }

        public static EditorDataTextureGroup GetGroup(string groupName)
        {
            foreach (var group in GetGroups())
            {
                if (group._groupName == groupName)
                    return group;
            }

            return null;
        }
        
        public void Setup(bool generateEditorLayout = false)
        {
            LoadTexturesFromFolder(false);

            EditorUtility.SetDirty(this);
            
            if (generateEditorLayout)
                EditorTexturesGenerator.Run();
        }

        public void LoadTexturesFromFolder(bool saveAssets = true)
        {
            _textures.Clear();
            string assetPath = AssetDatabase.GetAssetPath(this);
            string assetParentFolderPath = assetPath.Replace($"{name}.asset", "");
            string[] files = Directory.GetFiles(assetParentFolderPath, "*.png", SearchOption.TopDirectoryOnly);
            
            if (files.Length == 0)
            {
                return;
            }
            
            TextureUtils.SetTextureSettingsToGUI(files);
            List<Texture2D> textures2D = TextureUtils.GetTextures2D(assetParentFolderPath);
            foreach (Texture2D texture2D in textures2D)
                _textures.Add(new EditorTextureInfo
                {
                    TextureReference = texture2D
                });

            Validate(saveAssets);
        }

        internal void Validate(bool saveAssets = true)
        {
            string path = AssetDatabase.GetAssetPath(this);
            string[] splitPath = path.Split('/');
            string folderName = splitPath[splitPath.Length - 2];

            _groupCategory = _groupCategory.RemoveWhitespaces().RemoveAllSpecialCharacters();
            _groupName = folderName.RemoveWhitespaces().RemoveAllSpecialCharacters();
            
            AssetDatabase.RenameAsset(path, $"{DefaultAssetFilename}_{_groupName}_{GroupCategory}");

            _textures = _textures ?? new List<EditorTextureInfo>();
            
            RemoveNullEntries();
            RemoveDuplicates();
            
            foreach (EditorTextureInfo textureInfo in _textures)
            {
                string textureFileName = textureInfo.TextureReference.name;
                if (!_removePrefixFromTextureName.IsNullOrEmpty())
                    textureFileName = textureFileName.Replace(_removePrefixFromTextureName, "");
                textureInfo.TextureName = textureFileName;
                textureInfo.ValidateName();
            }

            EditorUtility.SetDirty(this);
            if (saveAssets) AssetDatabase.SaveAssets();
        }

    }
}
#endif