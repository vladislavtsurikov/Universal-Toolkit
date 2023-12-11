using System;

namespace VladislavTsurikov.UIElementsUtility.Runtime.Color
{
    [Serializable]
    public class ThemeColor
    {
        public virtual bool IsDarkTheme { get; set; }

        public UnityEngine.Color ColorOnDark;
        public UnityEngine.Color ColorOnLight;

        public UnityEngine.Color color =>
            IsDarkTheme
                ? ColorOnDark
                : ColorOnLight;

        public ThemeColor()
        {
            ColorOnDark = ColorOnLight = UnityEngine.Color.white;
        }
    }
}