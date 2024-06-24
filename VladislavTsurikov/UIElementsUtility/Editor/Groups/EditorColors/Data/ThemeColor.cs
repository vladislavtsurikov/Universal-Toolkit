#if UNITY_EDITOR
using System;

namespace VladislavTsurikov.UIElementsUtility.Editor.Groups.EditorColors
{
    [Serializable]
    public class ThemeColor
    {
        public virtual bool IsDarkTheme { get; set; }

        public UnityEngine.Color ColorOnDark;
        public UnityEngine.Color ColorOnLight;

        public UnityEngine.Color Color =>
            IsDarkTheme
                ? ColorOnDark
                : ColorOnLight;


        public ThemeColor()
        {
            ColorOnDark = ColorOnLight = UnityEngine.Color.white;
        }
    }
}
#endif