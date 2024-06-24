using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Core;

namespace VladislavTsurikov.IMGUIUtility.Runtime.ElementStack.IconStack
{
    public interface IShowIcon : ISelected
    {
#if UNITY_EDITOR
        string Name { get; }
        bool IsRedIcon { get; }
        
        Texture2D PreviewTexture { get; }
#endif
    }
}