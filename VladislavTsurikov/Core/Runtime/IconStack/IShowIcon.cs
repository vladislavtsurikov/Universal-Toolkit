using UnityEngine;
using VladislavTsurikov.Core.Runtime.Interfaces;

namespace VladislavTsurikov.Core.Runtime.IconStack
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