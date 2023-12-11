#if UNITY_EDITOR
using System;
using UnityEngine;
using VladislavTsurikov.Utility.Runtime.Extensions;

namespace VladislavTsurikov.UIElementsUtility.Editor.EditorUI.ScriptableObjects.Textures
{
    [Serializable]
    public class EditorTextureInfo
    {
        public string TextureName;
        public Texture2D TextureReference;
        
        public void ValidateName() =>
            TextureName = TextureName.RemoveWhitespaces().RemoveAllSpecialCharacters();
    }
}
#endif
