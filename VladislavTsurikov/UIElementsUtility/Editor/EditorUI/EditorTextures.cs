//.........................
//.....Generated File......
//.........................
//.......Do not edit.......
//.........................

#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.UIElementsUtility.Editor.EditorUI.ScriptableObjects.Textures;

namespace VladislavTsurikov.UIElementsUtility.Editor.EditorUI
{
    public static class EditorTextures
    {
        public static class Images
        {
            public enum TextureName
            {
                Minus,
                Plus
            }

            private static EditorDataTextureGroup s_textureGroup;
            private static Texture2D s_Minus;
            private static Texture2D s_Plus;

            private static EditorDataTextureGroup textureGroup { get; } = s_textureGroup != null? s_textureGroup: s_textureGroup = EditorDataTextureGroup.GetGroup("Images");

            public static Texture2D Minus { get; } = s_Minus ? s_Minus : s_Minus = GetTexture2D(TextureName.Minus);

            public static Texture2D Plus { get; } = s_Plus ? s_Plus : s_Plus = GetTexture2D(TextureName.Plus);

            private static Texture2D GetTexture2D(TextureName textureName)
            {
                return textureGroup.GetTexture(textureName.ToString());
            }
        }
    }
}
#endif
