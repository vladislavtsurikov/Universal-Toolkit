﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VladislavTsurikov.UIElementsUtility.Runtime.Core;
using VladislavTsurikov.Utility.Runtime;
#if UNITY_EDITOR
using UnityEditor;
using VladislavTsurikov.UIElementsUtility.Editor.Groups.Textures;
#endif

namespace VladislavTsurikov.UIElementsUtility.Runtime.Groups.Textures
{
    [
        CreateAssetMenu
        (
            fileName = "TextureGroup", 
            menuName = "VladislavTsurikov/UIElementsUtility/Texture Group"
        )
    ]
    public class TextureGroup : FilesDataGroup<TextureGroup, TextureInfo>
    {
        [SerializeField] 
        private string _groupCategory;
        public string GroupCategory => _groupCategory;

        [SerializeField] 
        private string _removePrefixFromTextureName;
        public string RemovePrefixFromTextureName => _removePrefixFromTextureName;

        public void AddNewItem()
        {
            _items.Insert(0, new TextureInfo());
        }

        public void RemoveNullEntries()
        {
            _items = _items.Where(item => item != null && item.TextureReference != null).ToList();
        }

        public void SortByFileName()
        {
            _items = _items.OrderBy(item => item.TextureReference.name).ToList();
        }

        public void RemoveDuplicates()
        {
            _items = _items.GroupBy(item => item.TextureReference).Select(item => item.First()).ToList();
        }

        internal Texture2D GetTexture(string textureName)
        {
            string cleanName = textureName.RemoveWhitespaces().RemoveAllSpecialCharacters();

            foreach (TextureInfo textureInfo in _items.Where(ti => ti.TextureName.Equals(cleanName)))
            {
                return textureInfo.TextureReference;
            }
            
            Debug.LogWarning($"Texture '{textureName}' not found! Returned null.");
            
            return null;
        }

#if UNITY_EDITOR
        public override string GetFileFormat()
        {
            return "png";
        }

        protected override void AddItems(string[] files)
        {
            foreach (string filePath in files)
            {
                Texture2D reference = AssetDatabase.LoadAssetAtPath<Texture2D>(filePath);
                if (reference == null)
                {
                    continue;
                }
                
                _items.Add(new TextureInfo
                {
                    TextureReference = reference
                });
            }
        }
        
        internal override void ValidateGroup()
        {
            string assetPath = AssetDatabase.GetAssetPath(this);
            
            AssetDatabase.RenameAsset(assetPath, $"{AssetDefaultName}_{GroupName}_{GroupCategory}");
        }
        
        internal override void ValidateItems()
        {
            _items ??= new List<TextureInfo>();
            
            RemoveNullEntries();
            RemoveDuplicates();
            
            foreach (TextureInfo textureInfo in _items)
            {
                string filePath = AssetDatabase.GetAssetPath(textureInfo.TextureReference);
                TextureUtility.ReimportTextureToGUITexture(filePath);
                
                string textureFileName = textureInfo.TextureReference.name;
                if (!_removePrefixFromTextureName.IsNullOrEmpty())
                {
                    textureFileName = textureFileName.Replace(_removePrefixFromTextureName, "");
                }
                textureInfo.TextureName = textureFileName;
                textureInfo.ValidateName();
            }
        }
#endif
    }
}
