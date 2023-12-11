#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.RendererStack.Runtime.Core.RendererSystem;

namespace VladislavTsurikov.RendererStack.Editor.Core.RendererSystem
{
    public class RendererEditor : IMGUIElementEditor
    {
	    private Vector2 _windowScrollPos;
	    public CustomRenderer CustomRendererTarget => (CustomRenderer)Target;

	    public virtual RendererMenu GetRendererMenu()
		{
			return null;
		}
    }
}
#endif