#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace VladislavTsurikov.Utility.Runtime
{
    public static class GUIUtility 
    {
        public static Texture2D GetPrefabPreviewTexture(GameObject prefab)
        {
#if UNITY_EDITOR
            Texture2D previewTexture;

            if((previewTexture = AssetPreview.GetAssetPreview(prefab)) != null)
			{
				return previewTexture;
			}
                
			return AssetPreview.GetMiniTypeThumbnail(typeof(GameObject));
#else
            return Texture2D.whiteTexture;
#endif
        }

        public static void ContextMenuCallback(object obj)
        {
            if (obj != null && obj is Action)
                (obj as Action).Invoke();
        }

        public static bool IsModifierDown(EventModifiers modifiers)
        {
            Event e = Event.current;
            
            if ((e.modifiers & EventModifiers.FunctionKey) != 0)
                return false;

            EventModifiers mask = EventModifiers.Alt | EventModifiers.Control | EventModifiers.Shift | EventModifiers.Command;
            modifiers &= mask;

            if (modifiers == 0 && (e.modifiers & (mask & ~modifiers)) == 0)
                return true;

            if ((e.modifiers & modifiers) != 0 && (e.modifiers & (mask & ~modifiers)) == 0)
                return true;

            return false;
        }
    }
}
#endif
